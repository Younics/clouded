namespace Clouded.Auth.Shared.User;

public class UserPermissionInput
{
    public IEnumerable<object> PermissionIds { get; set; } = Array.Empty<object>();
    public object? OrganizationId { get; set; }
}
