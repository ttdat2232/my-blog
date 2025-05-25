using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Categories;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Blogs;

namespace MyBlog.Application.Commands.Blogs.CreateBlog;

public class CreateBlogCommandHandler(IBlogService _blogService, IUnitOfWork _unitOfWork)
    : IRequestHandler<CreateBlogCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateBlogCommand request,
        CancellationToken cancellationToken
    )
    {
        var blogResult = await _blogService.CreateBlogAsync(
            request.Title,
            request.Content,
            request.AuthorId,
            request.CategoryId,
            request.IsDraft,
            request.PublishDate,
            cancellationToken
        );

        if (blogResult.IsFailure)
            return Result<Guid>.Failure(blogResult.Error);

        var blog = blogResult.Data;
        await _unitOfWork.Repository<BlogAggregate, BlogId>().AddAsync(blog, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result<Guid>.Success(blog.Id);
    }
}
