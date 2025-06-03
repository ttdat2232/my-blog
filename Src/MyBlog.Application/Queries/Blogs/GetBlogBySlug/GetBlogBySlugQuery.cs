using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Models.Blogs;

namespace MyBlog.Application.Queries.Blogs.GetBlogBySlug;

public record GetBlogBySlugQuery(string Slug) : IRequest<Result<BlogResponse>>;
