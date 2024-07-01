namespace Clouded.Core.Mail.Library.Options;

public class MailOptions
{
    public string? FromEmailDefault { get; set; }
    public MailClientOptions Client { get; set; } = new();
}
