using Clouded.Function.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Clouded.Function.Api.Security;

public class CloudedAuthorizationFilter(ApplicationOptions options) : IAuthorizationFilter
{
    private readonly string _apiKey = options.Clouded.Function.ApiKey;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthorized = _apiKey == context.HttpContext.Request.Headers["X-CLOUDED-KEY"];

        if (!isAuthorized)
            context.Result = new UnauthorizedResult();
    }
}
