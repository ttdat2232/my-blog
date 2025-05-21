using MyBlog.Core.Aggregates.Blogs;

namespace MyBlog.WebApi.Models.Blogs;

public class BlogParamQuery
{
    public string? Title { get; set; }
    public BlogStatus? Status { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}
