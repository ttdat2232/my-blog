using MediatR;
using Microsoft.AspNetCore.Mvc;
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
}
