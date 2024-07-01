namespace Clouded.Auth.Provider.Options;

public class IdentityOptions
{
    public required EntityIdentityDomainOptions Domain { get; set; }
    public required EntityIdentityMachineOptions? Machine { get; set; }
    public EntityIdentityOrganizationOptions? Organization { get; set; }
    public required EntityIdentityUserOptions User { get; set; }
    public EntityIdentityRoleOptions? Role { get; set; }
    public EntityIdentityPermissionOptions? Permission { get; set; }
}
