namespace Clouded.Platform.App.Web.Options;

public class FunctionOptions
{
    public string RepositoryRedirectUrl { get; set; } = null!;
    public FunctionRepositoriesOptions Repositories { get; set; } = new();
}
