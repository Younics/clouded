using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Models.Dtos.Provider;

public class FunctionBuildInput
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public ERegionCode HubRegionCode { get; set; }
    public DateTime DeployAt { get; set; }
    public string Image { get; set; } = null!;
    public string GitRepositoryToken { get; set; } = null!;
    public string GitRepositoryUrl { get; set; } = null!;
    public string GitRepositoryBranch { get; set; } = null!;
}
