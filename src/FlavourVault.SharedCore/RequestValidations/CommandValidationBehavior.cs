using FlavourVault.SharedCore.Results;
using FluentValidation;
using MediatR;

namespace FlavourVault.SharedCore.RequestValidations;
public sealed class CommandValidationBehavior<TRequest, TResponse> :
  IPipelineBehavior<TRequest, TResponse>
  where TRequest : IRequest<TResponse>
  where TResponse : ICommandResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public CommandValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();
                
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .Where(validationResult => !validationResult.IsValid && validationResult.Errors != null)
            .SelectMany(r => r.Errors)
            .Select(x => new ResultError(x.PropertyName, x.ErrorMessage))
            .ToList();

        if (failures.Count == 0)
            return await next();
        
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<int>.Invalid), new[] { typeof(List<ResultError>) });

            if (invalidMethod != null)
            {
                return (TResponse)invalidMethod.Invoke(null, new object[] { failures });
            }
        }
        else if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Invalid(failures);
        }

        return await next();
    }
}