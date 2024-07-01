using Flurl.Http;
using Harbor.Library.Dtos;

namespace Harbor.Library;

public class HarborProjectClient(string server, string username, string password)
    : BaseClient(server, username, password)
{
    private const string ProjectUrlSegment = "/v2.0/projects";

    public async Task Create(ProjectInput input, CancellationToken cancellationToken = default) =>
        await Request(ProjectUrlSegment)
            .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken);

    public async Task Delete(long id, CancellationToken cancellationToken = default) =>
        await Request(ProjectUrlSegment, id)
            .DeleteAsync(HttpCompletionOption.ResponseContentRead, cancellationToken);

    public async Task AddMember(
        object projectNameOrId,
        ProjectMemberAddInput input,
        CancellationToken cancellationToken = default
    )
    {
        await Request(ProjectUrlSegment, projectNameOrId, "members")
            .PostJsonAsync(input, HttpCompletionOption.ResponseContentRead, cancellationToken);
    }

    public async Task RemoveMember(
        long projectNameOrId,
        long memberId,
        CancellationToken cancellationToken = default
    )
    {
        await Request(ProjectUrlSegment, projectNameOrId, "members", memberId)
            .DeleteAsync(HttpCompletionOption.ResponseContentRead, cancellationToken);
    }
}
