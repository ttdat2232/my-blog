using MediatR;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Cache;

namespace MyBlog.Application.Commands.Blogs.AddView;

public class AddViewComandHandler(ICacheService _cacheService)
    : IRequestHandler<AddViewCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        AddViewCommand request,
        CancellationToken cancellationToken
    )
    {
        var viewCounts = await _cacheService.GetAndRemoveAsync<IDictionary<Guid, long>>(
            "blog:viewcount",
            cancellationToken
        );

        if (viewCounts == null)
        {
            viewCounts = new Dictionary<Guid, long>();
            viewCounts[request.blogId] = 1;
        }
        else if (viewCounts.TryGetValue(request.blogId, out var previousCount))
        {
            viewCounts[request.blogId] = previousCount + 1;
        }

        _ = _cacheService
            .SetAsync("blog:viewcount", viewCounts, TimeSpan.FromDays(1), cancellationToken)
            .ContinueWith(_ => Serilog.Log.Information("Successfully cached view count"));
        return Result<bool>.Success(true);
    }
}
