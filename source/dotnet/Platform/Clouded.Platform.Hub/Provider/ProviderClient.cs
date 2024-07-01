using System.Net;
using Clouded.Platform.Models.Dtos.Provider;
using Flurl;
using Flurl.Http;

namespace Clouded.Platform.Hub.Provider;

public static class ProviderClient
{
    public static async Task<bool> CreateAsync(
        string serverUrl,
        ProviderCreateInput input,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await serverUrl
                .AppendPathSegment("v1/provider/create")
                .WithHeader("X-CLOUDED-KEY", apiKey)
                .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken);

            return response.StatusCode == (int)HttpStatusCode.OK;
        }
        catch (FlurlHttpException ex)
        {
            // TODO: Log exception in debug
            return false;
        }
    }

    public static async Task<bool> StartAsync(
        string serverUrl,
        string name,
        Dictionary<string, string>? envs,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await serverUrl
                .AppendPathSegments("v1/provider", name, "start")
                .WithHeader("X-CLOUDED-KEY", apiKey)
                .PatchJsonAsync(envs, cancellationToken: cancellationToken);

            return response.StatusCode == (int)HttpStatusCode.OK;
        }
        catch (FlurlHttpException ex)
        {
            // TODO: Log exception in debug
            return false;
        }
    }

    public static async Task<bool> StopAsync(
        string serverUrl,
        string name,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await serverUrl
                .AppendPathSegments("v1/provider", name, "stop")
                .WithHeader("X-CLOUDED-KEY", apiKey)
                .PatchAsync(cancellationToken: cancellationToken);

            return response.StatusCode == (int)HttpStatusCode.OK;
        }
        catch (FlurlHttpException ex)
        {
            // TODO: Log exception in debug
            return false;
        }
    }

    public static async Task<bool> DeleteAsync(
        string serverUrl,
        string name,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await serverUrl
                .AppendPathSegments("v1/provider", name, "delete")
                .WithHeader("X-CLOUDED-KEY", apiKey)
                .DeleteAsync(HttpCompletionOption.ResponseContentRead, cancellationToken);

            return response.StatusCode == (int)HttpStatusCode.OK;
        }
        catch (FlurlHttpException ex)
        {
            // TODO: Log exception in debug
            return false;
        }
    }

    public static async Task<bool> FunctionBuildAsync(
        string serverUrl,
        FunctionBuildInput input,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var response = await serverUrl
                .AppendPathSegment("v1/function/build")
                .WithHeader("X-CLOUDED-KEY", apiKey)
                .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken);

            return response.StatusCode == (int)HttpStatusCode.OK;
        }
        catch (FlurlHttpException ex)
        {
            // TODO: Log exception in debug
            return false;
        }
    }
}
