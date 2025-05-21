using System.Linq.Expressions;

namespace MyBlog.Core.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    int? Skip { get; }
    int? Take { get; }
    bool IsTracking { get; }
    List<Expression<Func<T, object>>> OrderByList { get; }
    List<Expression<Func<T, object>>> OrderByDescendingList { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    Expression<Func<T, object>> Selector { get; }
}
