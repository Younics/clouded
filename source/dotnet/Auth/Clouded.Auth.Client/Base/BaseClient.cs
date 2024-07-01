using System.Text.Json;
using Clouded.Results;
using Clouded.Results.Exceptions;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Clouded.Auth.Client.Base;

public abstract class BaseClient
{
    protected readonly FlurlClient RestClient;
    private readonly string? _cloudedKey;

    protected BaseClient(string apiUrl, string? cloudedKey = null, bool untrustedSsl = false)
    {
        _cloudedKey = cloudedKey;

        RestClient = new FlurlClient(
            untrustedSsl
                ? new HttpClient(
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                    }
                )
                : new HttpClient(),
            apiUrl
        );

        RestClient.WithSettings(settings =>
        {
            settings.JsonSerializer = new DefaultJsonSerializer(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            );
        });
        RestClient.AfterCall(AfterCallAsync);
    }

    private static async Task AfterCallAsync(FlurlCall call)
    {
        await ErrorHandlerAsync(call);
    }

    protected IFlurlRequest AddCloudedKeyHeader(IFlurlRequest request) =>
        _cloudedKey != null ? request.WithHeader("X-CLOUDED-KEY", _cloudedKey) : request;

    private static async Task ErrorHandlerAsync(FlurlCall call)
    {
        if (call.Response.StatusCode < 400)
            return;

        if (call is { Exception: not null, Response: null })
            return;

        if (call.Response == null)
            throw new CloudedException("No response", CloudedException.GeneralI18NMessage, 500);

        try
        {
            var result = await call.Response.GetJsonAsync<CloudedExceptionResult>();
            throw result.ToCloudedException(call.Response.StatusCode);
        }
        catch (FlurlHttpException)
        {
            throw new CloudedException(
                await call.Response.GetStringAsync(),
                CloudedException.GeneralI18NMessage,
                call.Response.StatusCode
            );
        }
    }
}
