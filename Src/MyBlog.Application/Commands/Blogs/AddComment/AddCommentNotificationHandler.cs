using MediatR;
using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Services.Messages;

namespace MyBlog.Application.Commands.Blogs.AddComment;

public class AddCommentNotificationHandler(IMessageBroker _messageBroker)
    : INotificationHandler<BlogCommentAddedEvent>
{
    public async Task Handle(
        BlogCommentAddedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var message = new Message<BlogCommentAddedEvent> { Payload = notification };
        await _messageBroker.CreateExchangeAsync("myblog.blog", "topic", true);
        await _messageBroker.PublishAsync(
            message,
            "myblog.comment",
            "added",
            cancellationToken: cancellationToken
        );
    }
}
