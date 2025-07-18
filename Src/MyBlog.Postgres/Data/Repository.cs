using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyBlog.Core.Repositories;
using MyBlog.Core.Specifications;

namespace MyBlog.Postgres.Data;

public class Repository<T, TId> : IRepository<T, TId>
    where T : class
{
    private readonly DbSet<T> _dbSet;

    public Repository(MyBlogContext context)
    {
        _dbSet = context.Set<T>();
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
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

    public virtual Task<int> CountAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    )
    {
        return expression == null
            ? _dbSet.CountAsync(cancellationToken)
            : _dbSet.CountAsync(expression, cancellationToken);
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task<T?> FindBy(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        return _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public virtual Task<T?> FindById(TId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken).AsTask();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
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

    public virtual async Task<IEnumerable<TMapped>> GetAllAsync<TMapped>(
        Expression<Func<T, TMapped>> select,
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbSet.AsQueryable();
        if (expression != null)
        {
            query = query.Where(expression);
        }
        return await query.Select(select).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TMapped>> GetAsync<TMapped>(
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

    public virtual async Task<IEnumerable<TMapped>> GetAsync<TMapped>(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<T> query = specification.IsTracking ? _dbSet : _dbSet.AsNoTracking();

        if (specification.Criteria != null)
            query = query.Where(specification.Criteria);

        foreach (var include in specification.Includes)
            query = query.Include(include);

        foreach (var includeString in specification.IncludeStrings)
            query = query.Include(includeString);

        if (specification.OrderBy != null)
            query = query.OrderBy(specification.OrderBy);
        if (specification.OrderByDescending != null)
            query = query.OrderByDescending(specification.OrderByDescending);
        foreach (var orderBy in specification.OrderByList)
            query = ((IOrderedQueryable<T>)query).ThenBy(orderBy);
        foreach (var orderByDesc in specification.OrderByDescendingList)
            query = ((IOrderedQueryable<T>)query).ThenByDescending(orderByDesc);

        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);
        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        if (specification.Selector != null)
        {
            var projected = query.Select(specification.Selector);
            var result = await projected.ToListAsync(cancellationToken);
            return result.OfType<TMapped>();
        }

        return await query
            .ToListAsync(cancellationToken)
            .ContinueWith(t => t.Result.OfType<TMapped>());
    }

    public virtual async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? expression = null,
        IEnumerable<Expression<Func<T, object>>>? includes = null,
        IEnumerable<string>? includeStrings = null,
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

        if (includes != null && includes.Any())
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        if (includeStrings != null && includeStrings.Any())
        {
            foreach (var includeString in includeStrings)
            {
                query = query.Include(includeString);
            }
        }

        query = query.Skip(pageIndex * pageSize).Take(pageSize);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<TMapped?> GetOneAsync<TMapped>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TMapped>> select,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.Where(expression).Select(select).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual Task<bool> IsExisted(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    )
    {
        return _dbSet.AnyAsync(expression, cancellationToken);
    }

    public virtual async Task<bool> IsExistedAsync(
        TId id,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet.FindAsync([id], cancellationToken) != null;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
