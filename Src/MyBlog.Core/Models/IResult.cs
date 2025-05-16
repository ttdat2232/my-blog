namespace MyBlog.Core.Models;

public interface IResult
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    public object GetData();
}
