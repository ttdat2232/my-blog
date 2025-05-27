using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Models;
using MyBlog.Core.Repositories;

namespace MyBlog.Core.Services.Roles;

public interface IRoleRepository : IRepository<RoleAggregate, RoleId>
{
    Task<Result<IEnumerable<string>>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
