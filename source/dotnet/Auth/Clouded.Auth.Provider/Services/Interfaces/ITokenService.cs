using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Output;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface ITokenService
{
    public OAuthValidateOutput JwtTokenValidate(OAuthAccessTokenInput input);
    public TokenOutput JwtTokenGenerate(
        object? userId = null,
        object? machineId = null,
        double? expiresInSeconds = null,
        object? blocked = null,
        object? cloudedData = null
    );
    public TokenOutput RefreshTokenGenerate();
}
