namespace MyBlog.Core.Primitives;

public class BaseId : ValueObject
{
    protected BaseId(Guid value)
    {
        Value = value;
    }

    public static BaseId New() => new(Guid.NewGuid());

    public static BaseId From(Guid id) => new(id);

    public static BaseId From(string id) => new(Guid.Parse(id));

    public Guid Value { get; private set; }

    public static implicit operator Guid(BaseId id) => id.Value;

    public static implicit operator Guid?(BaseId? id) => id?.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
