using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.Models.Dtos.Hub;

public class HubProviderStopInput
{
    public long Id { get; set; }
    public EProviderType Type { get; set; }
}
