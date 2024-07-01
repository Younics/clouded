using System.Net;
using Clouded.Auth.Shared.Pagination;
using Clouded.Results;
using Clouded.Shared;
using Flurl.Http;

namespace Clouded.Auth.Client.Base;

public abstract class BaseManagementClient(
    string apiUrl,
    string? apiKey,
    string? secretKey,
    string? cloudedKey,
    bool untrustedSsl = false
) : BaseClient(apiUrl, cloudedKey, untrustedSsl)
{
    protected internal readonly string? ApiKey = apiKey;
    protected internal readonly string? SecretKey = secretKey;
    protected internal readonly string? CloudedKey = cloudedKey;

    protected async Task<CloudedOkResult<Paginated<Dictionary<string, object?>>>> GetPaginatedAsync(
        string urlPathSegment,
        int page,
        int size,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient
            .Request()
            .AppendPathSegment(urlPathSegment + "/paginated")
            .SetQueryParams(new { page, size });

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        return await request.GetJsonAsync<CloudedOkResult<Paginated<Dictionary<string, object?>>>>(
            HttpCompletionOption.ResponseContentRead,
            cancellationToken
        );
    }

    protected async Task<CloudedOkResult<Dictionary<string, object?>>> GetByIdAsync(
        string urlPathSegment,
        object id,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient.Request().AppendPathSegments(urlPathSegment, id);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        return await request.GetJsonAsync<CloudedOkResult<Dictionary<string, object?>>>(
            HttpCompletionOption.ResponseContentRead,
            cancellationToken
        );
    }

    protected async Task<CloudedOkResult<Dictionary<string, object?>>> CreateAsync(
        string urlPathSegment,
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient.Request().AppendPathSegments(urlPathSegment);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        return await request
            .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJson<CloudedOkResult<Dictionary<string, object?>>>();
    }

    protected async Task<CloudedOkResult<Dictionary<string, object?>>> UpdateAsync(
        string urlPathSegment,
        object id,
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient.Request().AppendPathSegments(urlPathSegment, id);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        return await request
            .PatchJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJson<CloudedOkResult<Dictionary<string, object?>>>();
    }

    protected async Task<bool> DeleteAsync(
        string urlPathSegment,
        object id,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient.Request().AppendPathSegments(urlPathSegment, id);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        var response = await request.DeleteAsync(
            HttpCompletionOption.ResponseContentRead,
            cancellationToken
        );
        return response.StatusCode == (int)HttpStatusCode.OK;
    }

    protected async Task<bool> AddOrRemoveAsync(
        string urlPathSegment,
        object id,
        string nextUrlPathSegment,
        object[] ids,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient
            .Request()
            .AppendPathSegments(urlPathSegment, id, nextUrlPathSegment);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        var response = await request.PostJsonAsync(
            ids,
            HttpCompletionOption.ResponseContentRead,
            cancellationToken
        );
        return response.StatusCode == (int)HttpStatusCode.OK;
    }

    protected IFlurlRequest AddMachineKeyHeader(IFlurlRequest request)
    {
        if (CloudedKey != null)
            return request;

        if (ApiKey == null || SecretKey == null)
            throw new ArgumentNullException(nameof(ApiKey));

        return request.WithHeader("X-CLOUDED-MACHINE-KEY", $"{ApiKey}:{SecretKey}".Base64Encode());
    }
}
