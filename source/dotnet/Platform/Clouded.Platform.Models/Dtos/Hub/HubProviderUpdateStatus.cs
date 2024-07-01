using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Models.Dtos.Hub;

public class HubProviderUpdateStatus
{
    public long Id { get; init; }
    public string? UserId { get; init; }
    public EProviderType Type { get; init; }
    public EProviderStatus Status { get; init; }
    public DateTime? DeployAt { get; set; }
}
