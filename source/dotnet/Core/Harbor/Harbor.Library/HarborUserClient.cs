using Flurl.Http;
using Harbor.Library.Dtos;

namespace Harbor.Library;

public class HarborUserClient(string server, string username, string password)
    : BaseClient(server, username, password)
{
    private const string UserUrlSegment = "/v2.0/users";

    public async Task<UserOutput> Current(CancellationToken cancellationToken = default) =>
        await Request(UserUrlSegment, "current")
            .GetJsonAsync<UserOutput>(HttpCompletionOption.ResponseContentRead, cancellationToken);

    public async Task Create(UserInput input, CancellationToken cancellationToken = default) =>
        await Request(UserUrlSegment)
            .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken);

    public async Task Delete(long id, CancellationToken cancellationToken = default) =>
        await Request(UserUrlSegment, id)
            .DeleteAsync(HttpCompletionOption.ResponseContentRead, cancellationToken);
}
