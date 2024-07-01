using Clouded.Platform.Hub.Options;
using Clouded.Shared;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace Clouded.Platform.Hub.Security;

public class CloudedAuthorizeFilter(ApplicationOptions options) : IHubFilter
{
    private readonly string _apiKey = options.Clouded.Hub.ApiKey;

    public ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        var httpContextFeature =
            invocationContext.Context.Features
                .SingleOrDefault(x => x.Key == typeof(IHttpContextFeature))
                .Value as IHttpContextFeature;

        var cloudedKey = httpContextFeature?.HttpContext?.GetBearerToken();

        if (_apiKey == cloudedKey)
            return next(invocationContext);

        throw new UnauthorizedAccessException();
    }
}
