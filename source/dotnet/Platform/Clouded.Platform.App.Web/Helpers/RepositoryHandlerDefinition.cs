using System.Text.RegularExpressions;
using Blazored.SessionStorage;
using Clouded.Function.Shared;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Microsoft.AspNetCore.Components;
using Octokit;

namespace Clouded.Platform.App.Web.Helpers;

public abstract class RepositoryHandlerDefinition(
    string name,
    string icon,
    UserIntegrationEntity? currentUserIntegration,
    CloudedOptions cloudedOptions,
    CloudedDbContext context,
    ISessionStorageService sessionStorageService,
    IUserIntegrationService userIntegrationService
)
{
    public readonly string Name = name;
    public readonly string Icon = icon;
    protected readonly CloudedOptions CloudedOptions = cloudedOptions;
    protected readonly CloudedDbContext Context = context;
    protected readonly ISessionStorageService SessionStorageService = sessionStorageService;
    protected readonly IUserIntegrationService UserIntegrationService = userIntegrationService;
    protected UserIntegrationEntity? CurrentUserIntegration = currentUserIntegration;

    public abstract bool IsRepositoryProviderConnected();
    public abstract Task Process(string uriQuery, CancellationToken cancellationToken);
    public abstract Task<IReadOnlyList<Repository>> GetRepositories();
    public abstract Task<IReadOnlyList<Branch>> GetBranches(long repositoryId);

    public abstract Task Connect(
        NavigationManager navigationManager,
        string projectId,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="branchName"></param>
    /// <returns></returns>
    /// <exception cref="FunctionConfigNotFoundException"></exception>
    /// <exception cref="GettingFunctionsConfigException"></exception>
    /// <exception cref="FunctionConfigInvalidException"></exception>
    public abstract Task<(
        List<FunctionProviderExecutableInput>,
        string?,
        FunctionProject?
    )> LoadFunctionsFromRepositoryBranch(Repository repository, string branchName);

    public abstract Task<
        IEnumerable<(string FunctionName, EFunctionType FunctionType)>
    > ScanForAnnotations(long repositoryId, string path, string reference);

    protected static IEnumerable<(string FunctionName, EFunctionType FunctionType)> FindAnnotations(
        string? content
    )
    {
        var annotations = new List<(string FunctionName, EFunctionType FunctionType)>();

        if (content == null)
            return annotations;

        var annotationMatches = Regex.Matches(
            content,
            @"\[.*?CloudedMap.*?\((.*?)\).*?\].*?class.*?:(.*?){",
            RegexOptions.Singleline
        );

        foreach (Match match in annotationMatches)
        {
            var nameMatch = Regex.Match(
                match.Groups[1].Value,
                @"(?<="").*?(?="")",
                RegexOptions.Singleline
            );

            var hookName = nameMatch.Value;
            var hookType = match.Groups[2].Value.Trim();

            annotations.Add(
                (FunctionName: hookName, FunctionType: hookType.AsEnum<EFunctionType>())
            );
        }

        return annotations;
    }
}
