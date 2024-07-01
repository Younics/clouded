using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Clouded.Platform.Provider.Options;

public class TokenOptions
{
    public string ValidIssuer { get; set; } = null!;
    public bool ValidateIssuer { get; set; }
    public string ValidAudience { get; set; } = null!;
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string Secret { get; set; } = null!;

    public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Secret));
}
