namespace MyBlog.Core.Models;

public class Result<T> : IResult
{
    private Result(bool isSuccess, T? data, Error error)
    {
        IsSuccess = isSuccess;
        _data = data;
        Error = error;
    }

    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; private set; }
    public T Data =>
        IsSuccess
            ? _data!
            : throw new ArgumentException("Failed to get result data becasue it was not success");
    private readonly T? _data;

    public static Result<T> Success(T data) => new(true, data, Error.None);

    public static Result<T> Failure(Error error) => new(false, default, error);

    public static Result<T> Failure(string description, int code) =>
        new(false, default, new(description, code));

    public object GetData() => Data!;
}
