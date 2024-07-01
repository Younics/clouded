using FluentEmail.Core.Models;

namespace Clouded.Core.Mail.Library.Services;

public interface IMailService
{
    public Task<SendResponse> SendEmailAsync(
        string toEmail,
        string subject,
        object context,
        string templateName,
        string? fromEmail = default,
        CancellationToken cancellationToken = default
    );
}
