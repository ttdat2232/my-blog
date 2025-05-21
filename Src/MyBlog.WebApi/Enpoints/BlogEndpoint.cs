using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Queries.Blogs.GetBlogs;
using MyBlog.WebApi.Extensions;
using MyBlog.WebApi.Models.Blogs;

namespace MyBlog.WebApi.Enpoints;

public class BlogEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/blogs");
        group.MapGet("", GetBlogs);
    }

    private static async Task<IActionResult> GetBlogs(
        [FromQuery] BlogParamQuery query,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var blogQuery = new GetBlogsQuery(
            query.Title,
            query.Status,
            query.PageNumber,
            query.PageSize
        );
        var result = await sender.Send(blogQuery, cancellationToken);
        return result.ToActionResult();
    }
}
