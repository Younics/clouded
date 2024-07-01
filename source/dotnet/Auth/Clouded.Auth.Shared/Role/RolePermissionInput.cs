namespace Clouded.Auth.Shared.Role;

public class RolePermissionInput
{
    public IEnumerable<object> PermissionIds { get; set; } = Array.Empty<object>();
}
