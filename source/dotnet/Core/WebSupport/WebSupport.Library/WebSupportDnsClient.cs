using System.Net;
using Flurl;
using Flurl.Http;
using WebSupport.Library.Dtos;

namespace WebSupport.Library;

public class WebSupportDnsClient(string apiUrl, string secret) : BaseClient(apiUrl, secret)
{
    private static string ZoneListUrlSegment(long userId) => $"/v1/user/{userId}/zone";

    private static string RecordMeCreateUrlSegment(string domain) =>
        $"/v1/user/self/zone/{domain}/record";

    private static string RecordMeDeleteUrlSegment(string domain, long recordId) =>
        $"/v1/user/self/zone/{domain}/record/{recordId}";

    private static string RecordCreateUrlSegment(long userId, string domain) =>
        $"/v1/user/{userId}/zone/{domain}/record";

    private static string RecordDeleteUrlSegment(long userId, string domain, long recordId) =>
        $"/v1/user/{userId}/zone/{domain}/record/{recordId}";

    public async Task<PaginationOutput<ZoneOutput>> Zones(
        long userId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var path = ZoneListUrlSegment(userId);

            var response = await ApiUri
                .AppendPathSegment(path)
                .WithHeaders(MakeHeaders(HttpMethod.Get, path))
                .GetJsonAsync<PaginationOutput<ZoneOutput>>(
                    HttpCompletionOption.ResponseContentRead,
                    cancellationToken
                );

            return response;
        }
        catch (FlurlHttpException ex)
        {
            if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                return new PaginationOutput<ZoneOutput>();

            throw;
        }
    }

    public Task<ActionOutput<RecordOutput>?> RecordCreate(
        string domain,
        RecordCreateInput input,
        CancellationToken cancellationToken = default
    ) => RecordCreate(null, domain, input, cancellationToken);

    public Task<ActionOutput<RecordOutput>?> RecordCreateForUser(
        long userId,
        string domain,
        RecordCreateInput input,
        CancellationToken cancellationToken = default
    ) => RecordCreate(userId, domain, input, cancellationToken);

    private async Task<ActionOutput<RecordOutput>?> RecordCreate(
        long? userId,
        string domain,
        RecordCreateInput input,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var path = userId.HasValue
                ? RecordCreateUrlSegment(userId.Value, domain)
                : RecordMeCreateUrlSegment(domain);

            var response = await ApiUri
                .AppendPathSegment(path)
                .WithHeaders(MakeHeaders(HttpMethod.Post, path))
                .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ReceiveJson<ActionOutput<RecordOutput>>();

            return response;
        }
        catch (FlurlHttpException ex)
        {
            if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                return null;

            throw;
        }
    }

    public Task<ActionOutput<RecordOutput>?> RecordDelete(
        string domain,
        long recordId,
        CancellationToken cancellationToken = default
    ) => RecordDelete(null, domain, recordId, cancellationToken);

    public Task<ActionOutput<RecordOutput>?> RecordDeleteForUser(
        long userId,
        string domain,
        long recordId,
        CancellationToken cancellationToken = default
    ) => RecordDelete(userId, domain, recordId, cancellationToken);

    private async Task<ActionOutput<RecordOutput>?> RecordDelete(
        long? userId,
        string domain,
        long recordId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var path = userId.HasValue
                ? RecordDeleteUrlSegment(userId.Value, domain, recordId)
                : RecordMeDeleteUrlSegment(domain, recordId);

            var response = await ApiUri
                .AppendPathSegment(path)
                .WithHeaders(MakeHeaders(HttpMethod.Delete, path))
                .DeleteAsync(HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ReceiveJson<ActionOutput<RecordOutput>>();

            return response;
        }
        catch (FlurlHttpException ex)
        {
            if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                return null;

            throw;
        }
    }
}
