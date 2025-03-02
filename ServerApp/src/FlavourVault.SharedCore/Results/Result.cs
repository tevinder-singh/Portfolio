using Microsoft.AspNetCore.Http;

namespace FlavourVault.SharedCore.Results;
public class Result: ICommandResult
{
    internal Result(ResultStatus resultStatus)
    {
        Status = resultStatus;
    }
    internal Result(ResultStatus resultStatus, string errorCode, string errorMessage) 
    { 
        Status = resultStatus;
        Errors = [new ResultError(errorCode, errorMessage)];
    }    

    public ResultStatus Status { get; private set; }
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.Created;    
    public IEnumerable<ResultError>? Errors { get; internal set; }

    public IDictionary<string, string[]> ToValidationErrors()
    {
        if (Errors == null)
            return new Dictionary<string, string[]>();

        return Errors
            .GroupBy(x => x.Identifier)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }

    public static Result Success() => new(ResultStatus.Ok);
    public static Result Failure(string errorCode, string errorMessage) => new(ResultStatus.Error, errorCode, errorMessage);
    public static Result NotFound() => new Result(ResultStatus.NotFound);
    public static Result Forbidden() => new(ResultStatus.Forbidden);
    public static Result Unauthorized() => new(ResultStatus.Unauthorized);
    public static Result Invalid(IEnumerable<ResultError> errors) => new(ResultStatus.Invalid) { Errors = errors };
}

public sealed class Result<T> : Result
{
    internal Result(T value): base(ResultStatus.Ok) => Value = value;
    internal Result(T value, ResultStatus resultStatus) : base(resultStatus) => Value = value;
    internal Result(ResultStatus resultStatus, string errorCode, string errorMessage) : base(resultStatus, errorCode, errorMessage) { }
    internal Result(ResultStatus resultStatus) : base(resultStatus) { }

    public T? Value { get; private set; }

    public IResult ToApiResult()
    {
        switch (Status)
        {
            case ResultStatus.Ok:
                return TypedResults.Ok(Value);
            case ResultStatus.Created:
                return TypedResults.Created("", Value);
            case ResultStatus.Error:
                return TypedResults.BadRequest(Errors);
            case ResultStatus.Forbidden:
                return TypedResults.Forbid();
            case ResultStatus.Unauthorized:
                return TypedResults.Unauthorized();
            case ResultStatus.NotFound:
                return TypedResults.NotFound();
            case ResultStatus.Invalid:
                return TypedResults.ValidationProblem(ToValidationErrors());
            default:
                return TypedResults.Ok();
        }
    }

    public static Result<T> Success(T data) => new(data);
    public static Result<T> Failure(string errorCode, string errorMessage) => new(ResultStatus.Error, errorCode, errorMessage);
    public static Result<T> Unauthorized() => new(ResultStatus.Unauthorized);
    public static Result<T> Forbidden() => new(ResultStatus.Forbidden);
    public static Result<T> Created(T data) => new(data, ResultStatus.Created);
    public static Result<T> Invalid(IEnumerable<ResultError> errors) => new(ResultStatus.Invalid) { Errors = errors };
}