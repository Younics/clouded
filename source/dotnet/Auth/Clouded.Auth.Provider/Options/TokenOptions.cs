using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Clouded.Auth.Provider.Options;

public class TokenOptions
{
    public bool ValidateLifetime { get; set; }
    public string ValidIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string Secret { get; set; }
    public double AccessTokenExpiration { get; set; }
    public double RefreshTokenExpiration { get; set; }

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Secret));
}
