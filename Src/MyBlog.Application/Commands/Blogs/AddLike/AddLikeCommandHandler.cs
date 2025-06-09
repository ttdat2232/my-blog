using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Commands.Blogs.AddLike;

public class AddLikeCommandHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<AddLikeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AddLikeCommand request,
        CancellationToken cancellationToken
    )
    {
        var blog = await _unitOfWork.BlogRepository.FindById(
            BlogId.From(request.BlogId),
            cancellationToken
        );
        if (blog == null)
            return Result<bool>.Failure(BlogErrors.NotFoundBlog);
        blog.AddLike(request.UserId);
        await _unitOfWork.SaveAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
