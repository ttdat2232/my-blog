using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users.Events;

public sealed record UserCreatedEvent(UserAggregate User) : DomainEvent;
