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
        var blog = await _unitOfWork
            .Repository<BlogAggregate, BlogId>()
            .GetOneAsync(
                b => b.Id == request.Id,
                b => new BlogResponse(
                    b.Id,
                    b.Title,
                    b.Content,
                    "TODO: Get Author Name", // You'll need to join with User table
                    "TODO: Get Category Name", // You'll need to join with Category table
                    b.CreatedAt,
                    b.PublishDate,
                    b.ViewCount
                ),
                cancellationToken
            );

        if (blog is null)
            return Result<BlogResponse>.Failure("Blog not found", 404);

        return Result<BlogResponse>.Success(blog);
    }
}
