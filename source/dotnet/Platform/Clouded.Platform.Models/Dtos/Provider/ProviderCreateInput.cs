using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Models.Dtos.Provider;

public class ProviderCreateInput
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public EProviderType Type { get; set; }
    public string UserId { get; set; } = null!;

    public string? CustomImage { get; set; }
    public Dictionary<string, string> Envs { get; set; } = new();
    public ERegionCode HubRegionCode { get; set; }
    public DateTime DeployAt { get; set; }
}
