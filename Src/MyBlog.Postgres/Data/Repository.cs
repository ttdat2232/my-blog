using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Repositories;

namespace MyBlog.Postgres.Data;

public class Repository<T, TId> : IRepository<T, TId>
    where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(MyBlogContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default
    )
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    )
    {
        return expression == null
            ? _dbSet.CountAsync(cancellationToken)
            : _dbSet.CountAsync(expression, cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<T?> FindBy(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        return _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public Task<T?> FindById(TId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync(new { id }, cancellationToken).AsTask();
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    )
    {
        var result =
            expression == null
                ? await _dbSet.ToListAsync(cancellationToken)
                : await _dbSet.Where(expression).ToListAsync(cancellationToken);
        return result.AsEnumerable();
    }

    public async Task<IEnumerable<TMapped>> GetAsync<TMapped>(
        Expression<Func<T, TMapped>> select,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int pageIndex = 0,
        int pageSize = 10,
        bool isTracked = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = isTracked ? _dbSet : _dbSet.AsNoTracking();

        if (expression != null)
            query = query.Where(expression);

        if (orderBy != null)
            query = orderBy(query);

        query = query.Skip(pageIndex * pageSize).Take(pageSize);

        return await query.Select(select).ToListAsync(cancellationToken);
    }

    public async Task<TMapped?> GetOneAsync<TMapped>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TMapped>> select,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.Where(expression).Select(select).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsExisted(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        return _dbSet.AnyAsync(expression, cancellationToken);
    }

    public async Task<bool> IsExistedAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken) != null;
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
