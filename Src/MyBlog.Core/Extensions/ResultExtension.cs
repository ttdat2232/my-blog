using MyBlog.Core.Models;

namespace MyBlog.Core.Extensions;

public static class ResultExtension
{
    public static T Match<T>(this Result<T> result, Func<T> onSuccess, Func<Error, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }
}
