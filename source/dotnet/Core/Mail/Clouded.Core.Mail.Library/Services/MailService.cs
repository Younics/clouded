using FluentEmail.Core;
using FluentEmail.Core.Models;

namespace Clouded.Core.Mail.Library.Services;

public class MailService(IFluentEmailFactory fluentEmailFactory) : IMailService
{
    public async Task<SendResponse> SendEmailAsync(
        string toEmail,
        string subject,
        object context,
        string template,
        string? fromEmail = default,
        CancellationToken cancellationToken = default
    )
    {
        var email = fluentEmailFactory.Create();

        if (fromEmail != null)
            email.SetFrom(fromEmail);

        return await email
            .To(toEmail)
            .Subject(subject)
            .UsingTemplate(template, context)
            .SendAsync(cancellationToken);
    }
}
