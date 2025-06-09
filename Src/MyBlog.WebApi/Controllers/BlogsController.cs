using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Application.Commands.Blogs.AddComment;
using MyBlog.Application.Commands.Blogs.AddLike;
using MyBlog.Application.Commands.Blogs.CreateBlog;
using MyBlog.Application.Queries.Blogs.GetBlogById;
using MyBlog.Application.Queries.Blogs.GetBlogBySlug;
using MyBlog.Application.Queries.Blogs.GetBlogs;
using MyBlog.WebApi.Extensions;
using MyBlog.WebApi.Models.Blogs;
using MyBlog.WebApi.Models.Blogs.Comments;

namespace MyBlog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogsController(ISender _sender) : ControllerBase
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
        var result = await _sender.Send(blogQuery, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id}/comments")]
    [Authorize("User")]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddCommentCommand(
            id,
            request.Content,
            User.GetUserId(),
            request.ParentCommentId
        );
        var result = await _sender.Send(command, cancellationToken);
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
        var result = await _sender.Send(createBlogCommand, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlogById(Guid id, CancellationToken cancellationToken)
    {
        var blogQuery = new GetBlogByIdQuery(id);
        var result = await _sender.Send(blogQuery, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBlogBySlug(string slug, CancellationToken cancellationToken)
    {
        var blogQuery = new GetBlogBySlugQuery(slug);
        var result = await _sender.Send(blogQuery, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{blogId}/like")]
    public async Task<IActionResult> LikeBlog(Guid blogId, CancellationToken cancellationToken)
    {
        var command = new AddLikeCommand(User.GetUserId(), blogId);
        var result = await _sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}
