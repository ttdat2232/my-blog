using MediatR;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Application.Queries.Blogs.GetBlogById;

public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, Result<BlogResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBlogByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BlogResponse>> Handle(
        GetBlogByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var blog = await _unitOfWork.BlogRepository.FindById(
            BlogId.From(request.Id),
            cancellationToken
        );

        if (blog is null)
            return Result<BlogResponse>.Failure("Blog not found", 404);

        return Result<BlogResponse>.Success(
            new(blog.Id, blog.Title, blog.Content, "", "", blog.CreatedAt, blog.PublishDate, 0)
        );
    }
}
