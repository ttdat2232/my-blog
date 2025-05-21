namespace MyBlog.Application.Models;

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int Count { get; set; }

    public PaginationResult(IEnumerable<T> items, int pageSize, int pageNumber, int count)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Count = count;
    }

    public static PaginationResult<T> New(
        IEnumerable<T> items,
        int pageSize,
        int pageNumber,
        int count
    )
    {
        var itemCount = items.Count();
        if (itemCount < pageSize)
            pageSize = itemCount;

        return new(items, pageSize, pageNumber, count);
    }
}
