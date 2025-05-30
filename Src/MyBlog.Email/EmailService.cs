using System.Net.Mail;
using Microsoft.Extensions.Options;
using MyBlog.Core.Models;
using MyBlog.Core.Services.Email;
using MyBlog.Email.Configurations;

namespace MyBlog.Email;

public class EmailService(IOptions<SmtpSettings> _smtpSettingsOptions) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = _smtpSettingsOptions.Value;

    public async Task<Result<bool>> SendEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            Serilog.Log.Debug(
                "Attempting to send email. To: {To}, Subject: {Subject}, IsHtml: {IsHtml}",
                to,
                subject,
                isHtml
            );

            using var client = new SmtpClient();
            client.Host = _smtpSettings.Host;
            client.Port = _smtpSettings.Port;
            client.EnableSsl = _smtpSettings.EnableSsl;
            client.Credentials = new System.Net.NetworkCredential(
                _smtpSettings.Username,
                _smtpSettings.Password
            );

            Serilog.Log.Debug(
                "Configured SMTP client. Host: {Host}, Port: {Port}, EnableSsl: {EnableSsl}",
                _smtpSettings.Host,
                _smtpSettings.Port,
                _smtpSettings.EnableSsl
            );

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };

            mailMessage.To.Add(to);

            if (cc?.Any() == true)
            {
                Serilog.Log.Debug("Adding CC recipients: {CC}", string.Join(", ", cc));
                foreach (var address in cc)
                {
                    mailMessage.CC.Add(address);
                }
            }

            if (bcc?.Any() == true)
            {
                Serilog.Log.Debug("Adding BCC recipients: {BCC}", string.Join(", ", bcc));
                foreach (var address in bcc)
                {
                    mailMessage.Bcc.Add(address);
                }
            }

            await client.SendMailAsync(mailMessage, cancellationToken);
            Serilog.Log.Information("Email sent successfully to {To}", to);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Failed to send email to {To}. Error: {Error}", to, ex.Message);
            return Result<bool>.Failure(new Error("Failed to send email", 500));
        }
    }
}
