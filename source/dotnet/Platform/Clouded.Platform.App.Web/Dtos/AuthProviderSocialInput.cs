namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderSocialInput
{
    public long? Id { get; set; }
    public bool Enabled { get; set; }
    public string? Key { get; set; }
    public string? Secret { get; set; }
    public string? RedirectUrl { get; set; }
    public string? DeniedRedirectUrl { get; set; }
}
