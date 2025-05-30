using MyBlog.Core.Aggregates.Blogs.Events;
using MyBlog.Core.Aggregates.Users;
using MyBlog.Core.Models.Messages;
using MyBlog.Core.Repositories;
using MyBlog.Core.Services.Email;
using MyBlog.Core.Services.Messages;

namespace MyBlog.BackgroundServices.Services;

public class BlogCreatedEventHandler(IServiceProvider _serviceProvider) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Information("BlogCreatedEventHandler started.");
        return ExcuteAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Information("BlogCreatedEventHandler stopped.");
        return Task.CompletedTask;
    }

    private async Task ExcuteAsync(CancellationToken cancellationToken)
    {
        Serilog.Log.Debug("BlogCreatedEventHandler executing...");
        using var scope = _serviceProvider.CreateScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var queueName = "handle.created.queue";
        await messageBroker.CreateQueueAsync(queueName);
        await messageBroker.BindQueueAsync(queueName, "myblog.blog", "created");
        await messageBroker.SubscribeAsync<BlogCreatedEvent>(
            queueName,
            msg => Handle(msg, unitOfWork, emailService, cancellationToken),
            autoAck: true,
            cancellationToken: cancellationToken
        );
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public static async Task<bool> Handle(
        Message<BlogCreatedEvent> notification,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        CancellationToken cancellationToken
    )
    {
        Serilog.Log.Debug("BlogCreatedEvent received: {Title}", notification.Payload.Title);
        var userRepo = unitOfWork.Repository<UserAggregate, UserId>();
        var followers = await userRepo.GetAllAsync(
            select: u => new { u.Email, u.UserName },
            expression: u =>
                u.Follows != null
                && u.Follows.Any(f => f.FollowedId == notification.Payload.AuthorId),
            cancellationToken: cancellationToken
        );

        var subject = "New Blog Created";
        var body = $"A new blog has been created with the title: {notification.Payload.Title}";
        if (!followers.Any())
        {
            Serilog.Log.Debug(
                "No followers found for user: {UserId}",
                notification.Payload.AuthorId
            );
            return true; // No followers to notify, consider it a success
        }
        Serilog.Log.Debug(
            "Found {Count} followers for user: {UserId}",
            followers.Count(),
            notification.Payload.AuthorId
        );
        var sendingMailTasks = followers.Select(f =>
            emailService.SendEmailAsync(
                f.Email,
                subject,
                body,
                true,
                cancellationToken: cancellationToken
            )
        );
        var results = await Task.WhenAll(sendingMailTasks);
        return results.All(result => result.IsSuccess);
    }
}
