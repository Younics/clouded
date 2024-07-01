using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Models.Dtos.Hub;

public class HubProviderProcessingOutput
{
    public long Id { get; set; }
    public bool Success { get; set; }
    public EProviderType Type { get; set; }
    public string SuccessMessage { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
    public DateTime? DeployedAt { get; set; }
}
