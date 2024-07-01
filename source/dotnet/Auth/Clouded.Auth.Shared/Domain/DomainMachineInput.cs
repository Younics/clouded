namespace Clouded.Auth.Shared.Domain;

public class DomainMachineInput
{
    public IEnumerable<object> MachineIds { get; set; } = Array.Empty<object>();
}
