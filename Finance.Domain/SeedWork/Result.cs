namespace Finance.Domain.SeedWork;

public readonly struct Unit
{
    public static readonly Unit Value = new();
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; } = default!;
    public List<string> Errors { get; } = new();

    private Result(bool isSuccess, T value, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        if (errors != null)
            Errors = errors;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value);
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>(false, default!, errors);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default!, new List<string> { error });
    }
}

public class Result
{
    public static Result<T> Success<T>(T value)
    {
        return Result<T>.Success(value);
    }

    public static Result<T> Failure<T>(List<string> errors)
    {
        return Result<T>.Failure(errors);
    }

    public static Result<T> Failure<T>(string error)
    {
        return Result<T>.Failure(error);
    }
}
