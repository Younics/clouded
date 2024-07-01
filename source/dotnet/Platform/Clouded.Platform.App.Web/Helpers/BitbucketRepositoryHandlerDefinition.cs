using Blazored.SessionStorage;
using Clouded.Function.Shared;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Clouded.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Octokit;

namespace Clouded.Platform.App.Web.Helpers;

public class BitbucketRepositoryHandlerDefinition(
    UserIntegrationEntity currentUserIntegration,
    CloudedOptions cloudedOptions,
    CloudedDbContext context,
    ISessionStorageService sessionStorageService,
    IUserIntegrationService userIntegrationService
)
    : RepositoryHandlerDefinition(
        ERepositoryProvider.Bitbucket.GetEnumName(),
        "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M2.65 3C2.3 3 2 3.3 2 3.65v.12l2.73 16.5c.07.42.43.73.85.73h13.05c.31 0 .59-.22.64-.54L22 3.77a.643.643 0 0 0-.54-.73c-.03-.01-.07-.01-.11-.01L2.65 3M14.1 14.95H9.94L8.81 9.07h6.3l-1.01 5.88Z\"/></svg>",
        currentUserIntegration,
        cloudedOptions,
        context,
        sessionStorageService,
        userIntegrationService
    )
{
    public override bool IsRepositoryProviderConnected()
    {
        return false;
    }

    public override Task Process(string uriQuery, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override Task<IReadOnlyList<Repository>> GetRepositories()
    {
        throw new NotImplementedException();
    }

    public override Task<IReadOnlyList<Branch>> GetBranches(long repositoryId)
    {
        throw new NotImplementedException();
    }

    public override Task Connect(
        NavigationManager navigationManager,
        string projectId,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }

    public override Task<(
        List<FunctionProviderExecutableInput>,
        string?,
        FunctionProject?
    )> LoadFunctionsFromRepositoryBranch(Repository repository, string branchName)
    {
        throw new NotImplementedException();
    }

    public override Task<
        IEnumerable<(string FunctionName, EFunctionType FunctionType)>
    > ScanForAnnotations(long repositoryId, string path, string reference)
    {
        throw new NotImplementedException();
    }
}
