using Clouded.Core.Mail.Library.Options;
using Clouded.Core.Mail.Library.Services;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Clouded.Core.Mail.Library;

public static class ServiceExtensions
{
    public static IServiceCollection AddMailServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddScoped<IMailService, MailService>();

        var mailSection = configuration.GetSection("Clouded").GetSection("Mail");
        var mailOptions = mailSection.Get<MailOptions?>();
        services.Configure<MailOptions>(mailSection.Bind);

        var mailTemplatesPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Mail",
            "Templates"
        );

        Directory.CreateDirectory(mailTemplatesPath);

        var builder = services.AddFluentEmail(
            mailOptions?.FromEmailDefault ?? "no-reply@clouded.sk"
        );

        if (mailOptions == null)
            return services;

        builder
            .AddLiquidRenderer(options =>
            {
                options.FileProvider = new PhysicalFileProvider(mailTemplatesPath);
            })
            .AddMailKitSender(
                new SmtpClientOptions
                {
                    Server = mailOptions.Client.Server,
                    Port = mailOptions.Client.Port,
                    User = mailOptions.Client.User,
                    Password = mailOptions.Client.Password,
                    UseSsl = mailOptions.Client.UseSsl,
                    PreferredEncoding = mailOptions.Client.PreferredEncoding,
                    RequiresAuthentication = mailOptions.Client.RequiresAuthentication,
                    SocketOptions = mailOptions.Client.SocketOptions
                }
            );

        return services;
    }
}
