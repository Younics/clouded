using System.Security.Claims;
using Clouded.Auth.Provider.Management.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Clouded.Auth.Provider.Management.Providers;

public class ManagementAuthenticationStateProvider(IManagementAuthService authService)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var claimsIdentity = await authService.IsAuthenticatedAsync()
            ? new ClaimsIdentity(
                new Claim[] { new("user_id", "-1"), new("blocked", $"{false}") },
                "Basic"
            )
            : new ClaimsIdentity();

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        return new AuthenticationState(claimsPrincipal);
    }
}
