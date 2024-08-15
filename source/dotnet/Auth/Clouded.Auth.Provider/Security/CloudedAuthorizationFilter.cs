using Clouded.Auth.Provider.Exceptions;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Token.Input.Base;
using Clouded.Results.Exceptions;
using Clouded.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Security;

public class CloudedAuthorizationFilter(ApplicationOptions options, IAuthService authService)
    : IAuthorizationFilter
{
    private readonly string _apiKey = options.Clouded.Auth.ApiKey;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthorized = _apiKey == context.HttpContext.Request.Headers["X-CLOUDED-KEY"];
        if (isAuthorized)
            return;

        if (
            !context.HttpContext.Request.Headers.TryGetValue(
                "X-CLOUDED-MACHINE-KEY",
                out var base64Key
            )
        )
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var decodedKey = base64Key.ToString().Base64Decode().Split(":");
        var apiKey = decodedKey.FirstOrDefault();
        var secretKey = decodedKey.LastOrDefault();

        try
        {
            var machine = authService.CheckMachineKeysInput(
                new OAuthMachineKeysInput { ApiKey = apiKey, SecretKey = secretKey },
                null
            );

            if (machine == null)
            {
                context.Result = new UnauthorizedResult();
            }
        }
        catch (BadRequestException e)
        {
            context.Result = new UnauthorizedResult();
        }
        catch (UnauthorizedException e)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
