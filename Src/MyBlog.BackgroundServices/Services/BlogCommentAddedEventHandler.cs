using System.Reflection.Metadata;
using System.Text;
using MyBlog.Core.Aggregates.Blogs;
using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Email;
using MyBlog.Core.Services.Messages;

namespace MyBlog.BackgroundServices.Services;

public class BlogCommentAddedEventHandler(IServiceProvider _serviceProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("Starting BlogCommentAddedEventHandler...");

        return ExcecuteAsync(cancellationToken);
    }

    private async Task ExcecuteAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("Executing BlogCommentAddedEventHandler...");
        using var scope = _serviceProvider.CreateScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var queueName = "myblog.comment.added.handler.queue";
        await messageBroker.CreateQueueAsync(queueName);
        await messageBroker.BindQueueAsync(queueName, "myblog.comment", "added");
        await messageBroker.SubscribeAsync<BlogCommentAddedEvent>(
            queueName,
            msg => Handle(msg, unitOfWork, emailService, cancellationToken),
            autoAck: true,
            cancellationToken: cancellationToken
        );
        Serilog.Log.Debug("BlogCommentAddedEventHandler is now listening for events.");
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task<bool> Handle(
        Message<BlogCommentAddedEvent> msg,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        CancellationToken cancellationToken
    )
    {
        Serilog.Log.Debug(
            "Handling BlogCommentAddedEvent for BlogId: {BlogId}",
            msg.Payload.BlogId
        );
        var blogRepo = unitOfWork.Repository<BlogAggregate, BlogId>();
        var blog = await blogRepo.FindById(
            BlogId.From(msg.Payload.BlogId),
            cancellationToken: cancellationToken
        );
        if (blog == null)
        {
            Serilog.Log.Warning("Blog not found for BlogId: {BlogId}", msg.Payload.BlogId);
            return false;
        }

        var userRepo = unitOfWork.Repository<UserAggregate, UserId>();
        var author = await userRepo.FindById(
            UserId.From(msg.Payload.AuthorId),
            cancellationToken: cancellationToken
        );
        if (author == null)
        {
            Serilog.Log.Warning("Author not found for AuthorId: {AuthorId}", msg.Payload.AuthorId);
            return false;
        }
        var subject = "New Comment on Your Blog";
        var bodyHtmlBuilder = new StringBuilder();

        bodyHtmlBuilder.AppendLine(
            $"<h1>New Comment on Your Blog: {blog.Title} at {msg.Metadata.Timestamp.ToString("mm/dd/yyyy hh:mm:ss")}</h1>"
        );
        bodyHtmlBuilder.AppendLine($"<p>Author: {author.UserName} ({author.Email})</p>");
        bodyHtmlBuilder.AppendLine($"<p>Comment: {msg.Payload.Content}</p>");

        await emailService.SendEmailAsync(
            author.Email,
            subject,
            bodyHtmlBuilder.ToString(),
            cancellationToken: cancellationToken
        );

        return true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("Stopping BlogCommentAddedEventHandler...");

        return Task.CompletedTask;
    }
}
