using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Blogs.CreateBlog;
using MyBlog.Application.Queries.Blogs.GetBlogs;
using MyBlog.WebApi.Extensions;
using MyBlog.WebApi.Models.Blogs;

namespace MyBlog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBlogs(
        [FromQuery] BlogParamQuery query,
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

    [HttpPost]
    [Authorize("User")]
    public async Task<IActionResult> CreateBlog(
        [FromBody] CreateBlogRequest request,
        CancellationToken cancellationToken
    )
    {
        var createBlogCommand = new CreateBlogCommand(
            request.Title,
            request.Content,
            User.GetUserId(),
            request.CategoryId,
            request.PublishDate,
            request.IsDraft
        );
        var result = await sender.Send(createBlogCommand, cancellationToken);
        return result.ToActionResult();
    }
}
