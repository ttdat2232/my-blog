using MediatR;
using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Services.Messages;

namespace MyBlog.Application.Commands.Blogs.CreateBlog;

public class CreateBlogNotificationHandler(IMessageBroker _messageBroker)
    : INotificationHandler<BlogCreatedEvent>
{
    public async Task Handle(BlogCreatedEvent notification, CancellationToken cancellationToken)
    {
        Serilog.Log.Debug(
            "Setting up message for BlogCreatedEvent with title: {Title}",
            notification.Title
        );
        var message = new Message<BlogCreatedEvent> { Payload = notification };
        await _messageBroker.CreateExchangeAsync("myblog.blog", "topic", true);
        await _messageBroker.PublishAsync(
            message,
            "myblog.blog",
            "created",
            cancellationToken: cancellationToken
        );
    }
}
