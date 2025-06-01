using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Primitives;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Blogs;

namespace MyBlog.Application.Commands.Blogs.UpdateBlog;

public class UpdateBlogCommandHandler(IUnitOfWork _unitOfWork, IBlogService _blogService)
    : IRequestHandler<UpdateBlogCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateBlogCommand request,
        CancellationToken cancellationToken
    )
    {
        var blog = await _unitOfWork
            .Repository<BlogAggregate, BlogId>()
            .FindById(BlogId.From(request.Id), cancellationToken);

        if (blog is null)
            return Result<bool>.Failure("Blog not found", 404);

        var validationResult = await _blogService.ValidateUpdateOperationAsync(
            blog,
            BaseId.From(request.RequestUpdateUserId),
            request.CategoryId.HasValue ? BaseId.From(request.CategoryId.Value) : default,
            cancellationToken
        );

        if (validationResult.IsFailure)
            return validationResult;

        blog.Update(request.Title, request.Content, request.Status);

        await _unitOfWork.Repository<BlogAggregate, BlogId>().UpdateAsync(blog, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
