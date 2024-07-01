using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Clouded.Platform.Hub.Options;
using Microsoft.IdentityModel.Tokens;

namespace Clouded.Platform.Hub.Security;

public class CloudedSecurityTokenHandler(ApplicationOptions options) : ISecurityTokenValidator
{
    private readonly HubOptions _options = options.Clouded.Hub;

    public bool CanValidateToken { get; } = true;
    public int MaximumTokenSizeInBytes { get; set; } =
        TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

    public bool CanReadToken(string securityToken) => true;

    public ClaimsPrincipal ValidateToken(
        string securityToken,
        TokenValidationParameters validationParameters,
        out SecurityToken validatedToken
    )
    {
        validatedToken = new JwtSecurityToken();

        if (_options.ApiKey == securityToken)
        {
            var asd = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim("user_id", "-1"), new Claim(ClaimTypes.System, "true") },
                    "X-CLOUDED-KEY"
                )
            );
            return asd;
        }

        return new ClaimsPrincipal();
    }
}
