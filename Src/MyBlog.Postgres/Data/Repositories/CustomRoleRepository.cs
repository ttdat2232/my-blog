using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Aggregates.Roles;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Roles;

namespace MyBlog.Postgres.Data.Repositories;

public class CustomRoleRepository(MyBlogContext _context)
    : Repository<RoleAggregate, RoleId>(_context),
        IRoleRepository
{
    public async Task<Result<IEnumerable<string>>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        var roles = await _context
            .Set<RoleAggregate>()
            .Where(role =>
                _context.Set<UserRole>().Any(ur => ur.UserId == userId && ur.RoleId == role.Id)
            )
            .Select(role => role.NormalizeName)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<string>>.Success(roles);
    }
}
