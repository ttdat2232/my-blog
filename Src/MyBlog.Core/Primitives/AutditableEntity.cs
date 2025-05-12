namespace MyBlog.Core.Primitives;

public abstract class AutditableEntity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public long? CreatedBy { get; protected set; }
    public long? UpdatedBy { get; protected set; }

    public void Audit(bool isCreated, DateTime time)
    {
        if (isCreated)
            CreatedAt = time;
        UpdatedAt = time;
    }
}
