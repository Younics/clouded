namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderMailInput
{
    public long? Id { get; set; }
    public bool Enabled { get; set; }
    public string? From { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 25;
    public bool Authentication { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public bool UseSSL { get; set; }
    public string? SocketOptions { get; set; }
    public string ResetPasswordReturnUrl { get; set; }
}
