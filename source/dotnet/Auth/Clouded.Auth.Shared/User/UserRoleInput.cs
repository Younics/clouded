namespace Clouded.Auth.Shared.User;

public class UserRoleInput
{
    public IEnumerable<object> RoleIds { get; set; } = Array.Empty<object>();
    public object? OrganizationId { get; set; }
}
