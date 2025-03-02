using Microsoft.AspNetCore.Http;

namespace FlavourVault.Results;
public sealed class Result: ICommandResult
{
    private Result(ResultStatus resultStatus)
    {
        Status = resultStatus;        
    }

    private Result(ResultStatus resultStatus, string errorCode, string errorMessage) 
    { 
        Status = resultStatus;
        Errors = [new ResultError(errorCode, errorMessage)];
    }    

    public ResultStatus Status { get; private set; }
    
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.Created;
    
    public IEnumerable<ResultError> Errors { get; internal set; } = [];

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

    public IResult ToApiResult()
    {
        switch (Status)
        {
            case ResultStatus.Ok:
                return TypedResults.Ok();
            case ResultStatus.Created:
                return TypedResults.Created();
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

    #region static methods

    public static Result Success() => new(ResultStatus.Ok);
    public static Result<T> Success<T>(T data) => new(data);

    public static Result Failure(string errorCode, string errorMessage) => new(ResultStatus.Error, errorCode, errorMessage);
    public static Result<T> Failure<T>(string errorCode, string errorMessage) => new(ResultStatus.Error, errorCode, errorMessage);

    public static Result NotFound() => new Result(ResultStatus.NotFound);
    public static Result<T> NotFound<T>() => new(ResultStatus.NotFound);

    public static Result Forbidden() => new(ResultStatus.Forbidden);
    public static Result<T> Forbidden<T>() => new(ResultStatus.Forbidden);

    public static Result Unauthorized() => new(ResultStatus.Unauthorized);
    public static Result<T> Unauthorized<T>() => new(ResultStatus.Unauthorized);
    
    public static Result Invalid(IEnumerable<ResultError> errors) => new(ResultStatus.Invalid) { Errors = errors };
    public static Result<T> Invalid<T>(IEnumerable<ResultError> errors) => new(ResultStatus.Invalid) { Errors = errors };
    public static Result<T> ValidationFailure<T>(IEnumerable<ResultError> errors) => new(ResultStatus.Invalid) { Errors = errors };
    public static Result<T> Created<T>(T data) => new(data, ResultStatus.Created);

    #endregion
}

public sealed class Result<T> : ICommandResult
{
    internal Result(ResultStatus resultStatus)
    {
        Status = resultStatus;
    }

    internal Result(T value, ResultStatus resultStatus) : this(resultStatus)
    {
        Value = value;
    }

    internal Result(T value):this(value, ResultStatus.Ok) { }

    internal Result(ResultStatus resultStatus, string errorCode, string errorMessage):this(resultStatus)
    {
        Errors = [new ResultError(errorCode, errorMessage)];
    }

    public T Value { get; init; }

    public ResultStatus Status { get; private set; }
    
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.Created;
    
    public IEnumerable<ResultError> Errors { get; internal set; } = [];

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
}