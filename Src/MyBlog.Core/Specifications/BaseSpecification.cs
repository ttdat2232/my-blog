using System.Linq.Expressions;

namespace MyBlog.Core.Specifications;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; protected set; } = null;
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }
    public bool IsTracking { get; protected set; } = true;
    public Expression<Func<T, object>> OrderBy { get; protected set; } = null!;
    public Expression<Func<T, object>> OrderByDescending { get; protected set; } = null!;
    public List<Expression<Func<T, object>>> OrderByList { get; } = new();
    public List<Expression<Func<T, object>>> OrderByDescendingList { get; } = new();
    public Expression<Func<T, object>> Selector { get; protected set; } = null!;

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderByList.Add(orderByExpression);
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescendingList.Add(orderByDescExpression);
    }

    protected void ApplySelector(Expression<Func<T, object>> selector)
    {
        Selector = selector;
    }

    protected void AsNoTracking()
    {
        IsTracking = false;
    }
}
