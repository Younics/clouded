using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.PasswordReset;
using Clouded.Auth.Shared.Token;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Input.Base;
using Clouded.Auth.Shared.Token.Output;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IAuthService
{
    public OAuthValidateOutput Validate(OAuthAccessTokenInput input);
    public OAuthOutput Token(OAuthInput input);
    public void TokenRevoke(OAuthTokenRevokeInput input);
    public OAuthOutput TokenRefresh(OAuthTokenRefreshInput input);
    public TokenOutput TokenMachine(OAuthTokenMachineInput input);

    public Task ForgotPasswordAsync(
        object? identity,
        CancellationToken cancellationToken = default
    );

    public void ResetPassword(PasswordResetInput input);
    public IEnumerable<string> Permissions(OAuthAccessTokenInput input);
    public MachineDictionary? CheckMachineKeysInput(OAuthMachineKeysInput input, object? userId);

    public OAuthOutput FacebookToken(OAuthSocialInput input);
    public Task<Dictionary<string, object?>> FacebookMe(string code);
    public String GetFacebookRedirectUrl();

    public OAuthOutput GoogleToken(OAuthSocialInput input);
    public Task<Dictionary<string, object?>> GoogleMe(string code);
    public Task<string> GetGoogleRedirectUrl();

    public OAuthOutput AppleToken(OAuthSocialInput input);
    public void StoreAppleUser(string code, AppleUser userData);
    public Task<Dictionary<string, object?>> AppleMe(string code);
    public Task<string> GetAppleRedirectUrl();
}
