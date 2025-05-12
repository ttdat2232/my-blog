using MyBlog.Core.Primitives;

namespace MyBlog.Core.Aggregates.Users.Events;

public sealed class UserCreatedEvent(UserAggregate User) : DomainEvent;
