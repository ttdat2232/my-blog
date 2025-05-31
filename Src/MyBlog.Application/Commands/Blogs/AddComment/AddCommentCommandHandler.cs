using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Commands.Blogs.AddComment;

public class AddCommentCommandHandler(IUnitOfWork _unitOfWork)
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
        return Result<bool>.Success(true);
    }
}
