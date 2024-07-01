using System.Security.Claims;
using Clouded.Platform.App.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Clouded.Platform.App.Web.Providers;

public class CloudedAuthenticationStateProvider(IStorageService storageService)
    : AuthenticationStateProvider
{
    public async Task Login(string accessToken, string refreshToken)
    {
        await storageService.SaveTokens(accessToken, refreshToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await storageService.DeleteTokens();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claims = (await storageService.UserClaims()).ToArray();
        var identity = claims.Any() ? new ClaimsIdentity(claims, "Bearer") : new ClaimsIdentity();
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
