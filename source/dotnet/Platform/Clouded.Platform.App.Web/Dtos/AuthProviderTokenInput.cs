namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderTokenInput
{
    public long? Id { get; set; }
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public string? ValidIssuer { get; set; }
    public int AccessTokenExpiration { get; set; } = 86400;
    public int RefreshTokenExpiration { get; set; } = 432000;
}
