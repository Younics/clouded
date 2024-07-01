using System.Security.Claims;
using Clouded.Admin.Provider.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace Clouded.Admin.Provider.Providers;

public class AdminAuthenticationStateProvider(
    IAuthService authService,
    IStorageService storageService
) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await authService.IsAuthenticatedAsync()
            ? new ClaimsIdentity(await storageService.UserClaims(), "Basic")
            : new ClaimsIdentity();

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return new AuthenticationState(claimsPrincipal);
    }
}
