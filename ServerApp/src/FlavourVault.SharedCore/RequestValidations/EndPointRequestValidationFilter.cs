using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FlavourVault.SharedCore.RequestValidations;
public sealed class EndPointRequestValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    private readonly IValidator<TRequest> validator = validator;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
            return TypedResults.ValidationProblem([], $"Please provide value for {typeof(TRequest).Name}");

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!result.IsValid)        
            return TypedResults.ValidationProblem(result.ToDictionary());        

        return await next(context);
    }
}

public static class EndPointRequestValidationFilterExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<EndPointRequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();            
    }
}
