using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Cache;

namespace MyBlog.Application.Commands.Blogs.AddComment;

public class AddCommentCommandHandler(IUnitOfWork _unitOfWork, ICacheService _cacheService)
    : IRequestHandler<AddCommentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken
    )
    {
        var blogRepository = _unitOfWork.BlogRepository;

        var blog = await blogRepository.FindById(BlogId.From(request.BlogId), cancellationToken);
        if (blog is null)
        {
            return Result<bool>.Failure(BlogErrors.NotFoundBlog);
        }

        blog.AddComment(request.Content, request.AuthorId, request.ParentCommentId);
        await _unitOfWork.SaveAsync(cancellationToken);
        _ = _cacheService.RemoveAsync<BlogResponse>(
            ["id", blog.Id.Value.ToString()],
            cancellationToken
        );
        return Result<bool>.Success(true);
    }
}
