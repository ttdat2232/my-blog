using System.Linq.Expressions;
using MyBlog.Core.Primitives;
using MyBlog.Core.Specifications;

namespace MyBlog.Core.Repositories;

public interface IRepository<T, TId>
{
    Task<int> CountAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    );

    Task<T?> FindById(TId id, CancellationToken cancellationToken = default);
    Task<T?> FindBy(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );
    Task<bool> IsExistedAsync(TId id, CancellationToken cancellationToken = default);
    Task<bool> IsExisted(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default
    );
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<TMapped?> GetOneAsync<TMapped>(
        Expression<Func<T, bool>> expression,
        Expression<Func<T, TMapped>> select,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? expression = null,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TMapped>> GetAsync<TMapped>(
        Expression<Func<T, TMapped>> select,
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int pageIndex = 0,
        int pageSize = 10,
        bool isTracked = false,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<TMapped>> GetAsync<TMapped>(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default
    );

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}
