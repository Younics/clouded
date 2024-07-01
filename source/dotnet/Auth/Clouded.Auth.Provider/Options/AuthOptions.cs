using Clouded.Shared.Options;

namespace Clouded.Auth.Provider.Options;

public class AuthOptions
{
    public required string Name { get; set; }
    public required string ApiKey { get; set; }
    public AuthManagementOptions? Management { get; set; }
    public required SocialOptions Social { get; set; }
    public required CorsOptions Cors { get; set; }
    public required HashOptions Hash { get; set; }
    public required TokenOptions Token { get; set; }
    public required IdentityOptions Identity { get; set; }
}
