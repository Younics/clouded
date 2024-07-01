namespace Clouded.Auth.Shared.Machine;

public class MachinePermissionInput
{
    public IEnumerable<object> PermissionIds { get; set; } = Array.Empty<object>();
}
