using MyBlog.Core.Models;

namespace MyBlog.Core.Services.Email;

public interface IEmailService
{
    Task<Result<bool>> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        CancellationToken cancellationToken = default
    );
}
