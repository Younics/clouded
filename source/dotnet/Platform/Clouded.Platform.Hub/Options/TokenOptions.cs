using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Clouded.Platform.Hub.Options;

public class TokenOptions
{
    public string ValidIssuer { get; set; } = null!;
    public bool ValidateIssuer { get; set; }
    public string[] ValidAudiences { get; set; } = Array.Empty<string>();
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string Secret { get; set; } = null!;

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Secret));
}
