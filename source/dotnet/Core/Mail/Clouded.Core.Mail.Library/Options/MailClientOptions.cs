using MailKit.Security;

namespace Clouded.Core.Mail.Library.Options;

public class MailClientOptions
{
    public string? Server { get; set; }
    public int Port { get; set; } = 587;
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; }
    public string? PreferredEncoding { get; set; }
    public bool RequiresAuthentication { get; set; }
    public SecureSocketOptions SocketOptions { get; set; }
}
