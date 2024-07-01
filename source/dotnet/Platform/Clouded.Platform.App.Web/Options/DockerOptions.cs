namespace Clouded.Platform.App.Web.Options;

public class DockerOptions
{
    public string AuthProviderImage { get; set; } = null!;
    public string AdminProviderImage { get; set; } = null!;
    public string ApiProviderImage { get; set; } = null!;
    public string FunctionProviderImage { get; set; } = null!;
}
