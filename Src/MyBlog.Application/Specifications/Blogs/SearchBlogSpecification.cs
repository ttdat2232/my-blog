using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Specifications;

namespace MyBlog.Application.Specifications.Blogs;

public sealed class SearchBlogSpecification : BaseSpecification<BlogAggregate>
{
    public SearchBlogSpecification(string? title, BlogStatus? status, int pageNumber, int pageSize)
    {
        Criteria = b =>
            (string.IsNullOrWhiteSpace(title) || b.Title.Contains(title)) && b.Status == status;

        ApplyPaging(pageSize * (pageNumber - 1), pageSize);
        ApplyOrderByDescending(b => b.CreatedAt);
    }
}
