using System.Net;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Auth.Shared.Token.Output;
using Clouded.Results;
using Flurl.Http;

namespace Clouded.Auth.Client.Base;

public abstract class BaseAuthClient(
    string apiUrl,
    string? apiKey = null,
    string? secretKey = null,
    string? cloudedKey = null,
    bool untrustedSsl = false
) : BaseClient(apiUrl, cloudedKey, untrustedSsl)
{
    public readonly AuthManagementClient Management = new(apiUrl, apiKey, secretKey, cloudedKey);

    public async Task<CloudedOkResult<string>> FacebookLoginUrl(
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/facebook/login-url")
            .GetAsync(HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJson<CloudedOkResult<string>>();
    }

    public async Task<CloudedOkResult<Dictionary<string, object>>> FacebookMe(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/facebook/me/{code}")
            .GetJsonAsync<CloudedOkResult<Dictionary<string, object>>>(
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );
    }

    public async Task<CloudedOkResult<OAuthOutput>> FacebookToken(
        string code,
        object userId,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/facebook/token")
            .PostJsonAsync(
                new { code, userId },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<OAuthOutput>>();
    }

    public async Task<CloudedOkResult<string>> AppleLoginUrl(
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/apple/login-url")
            .GetAsync(HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJson<CloudedOkResult<string>>();
    }

    public async Task<CloudedOkResult<Dictionary<string, object>>> AppleMe(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/apple/me/{code}")
            .GetJsonAsync<CloudedOkResult<Dictionary<string, object>>>(
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );
    }

    public async Task<CloudedOkResult<OAuthOutput>> AppleToken(
        string code,
        object userId,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/apple/token")
            .PostJsonAsync(
                new { code, userId },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<OAuthOutput>>();
    }

    public async Task<CloudedOkResult<string>> GoogleLoginUrl(
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/google/login-url")
            .GetAsync(HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJson<CloudedOkResult<string>>();
    }

    public async Task<CloudedOkResult<Dictionary<string, object>>> GoogleMe(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/google/me/{code}")
            .GetJsonAsync<CloudedOkResult<Dictionary<string, object>>>(
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );
    }

    public async Task<CloudedOkResult<OAuthOutput>> GoogleToken(
        string code,
        object userId,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.SocialRoutePrefix}/google/token")
            .PostJsonAsync(
                new { code, userId },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<OAuthOutput>>();
    }

    public async Task<bool> Validate(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        var response = await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/token/validate")
            .PostJsonAsync(
                new { accessToken },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );

        return response.StatusCode == (int)HttpStatusCode.OK;
    }

    public async Task<CloudedOkResult<OAuthOutput>> Token(
        OAuthInput input,
        CancellationToken cancellationToken = default
    ) =>
        await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/token")
            .PostJsonAsync(
                new
                {
                    identity = input.Identity,
                    password = input.Password,
                    apiKey = Management.ApiKey,
                    secretKey = Management.SecretKey
                },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<OAuthOutput>>();

    public async Task<CloudedOkResult<OAuthOutput>> TokenRefresh(
        string accessToken,
        string refreshToken,
        CancellationToken cancellationToken = default
    ) =>
        await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/token/refresh")
            .PostJsonAsync(
                new
                {
                    accessToken,
                    refreshToken,
                    apiKey = Management.ApiKey,
                    secretKey = Management.SecretKey
                },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<OAuthOutput>>();

    public async Task<bool> TokenRevoke(
        string refreshToken,
        bool? allOfUser = false,
        CancellationToken cancellationToken = default
    )
    {
        var response = await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/token/revoke")
            .PostJsonAsync(
                new { refreshToken, allOfUser },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );

        return response.StatusCode == (int)HttpStatusCode.OK;
    }

    public async Task<CloudedOkResult<IEnumerable<String>>> Permissions(
        string accessToken,
        CancellationToken cancellationToken = default
    )
    {
        return await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/permissions")
            .PostJsonAsync(
                new { accessToken },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            )
            .ReceiveJson<CloudedOkResult<IEnumerable<String>>>();
    }

    public async Task<bool> ForgotPassword(
        string identity,
        CancellationToken cancellationToken = default
    )
    {
        var response = await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/forgot-password")
            .PostJsonAsync(
                new { identity },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );

        return response.StatusCode == (int)HttpStatusCode.OK;
    }

    public async Task<bool> ResetPassword(
        string resetToken,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var response = await RestClient
            .Request()
            .AppendPathSegment($"v1/{RoutesConfig.OAuthRoutePrefix}/reset-password")
            .PostJsonAsync(
                new { resetToken, password },
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            );

        return response.StatusCode == (int)HttpStatusCode.OK;
    }
}
