using Microsoft.AspNetCore.Mvc;
using MyBlog.Core.Models;

namespace MyBlog.WebApi.Extensions;

public static class ResultExtension
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        Func<T, object>? mapping = null
    )
    {
        if (result.IsSuccess)
        {
            var data = mapping != null ? mapping(result.Data!) : result.Data;
            return new ObjectResult(data) { StatusCode = 200 };
        }
        return new ObjectResult(new { Error = result.Error.ToString() })
        {
            StatusCode = result.Error.Code,
        };
    }
}
