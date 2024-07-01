using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Pagination;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IMachineDataSource : IDataSource<MachineDictionary>
{
    public MachineDictionary MachineFindByKeys(string apiKey, string secretKey);

    public Paginated<MachineDictionary> MachinePaginated(
        int page,
        int size,
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );

    public IEnumerable<MachineDictionary> Machines(
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );
    public IEnumerable<RoleDictionary> MachineRoles(object? machineId);
    public IEnumerable<PermissionDictionary> MachinePermissions(object? machineId);
    public IEnumerable<DomainDictionary> MachineDomains(object? machineId);
    public IEnumerable<PermissionDictionary> MachinePermissionsWithRolePermissions(
        object? machineId
    );
    public void MachineExtendWithRoles(MachineDictionary machine);
    public void MachineExtendWithPermissions(MachineDictionary machine);
    public void MachineExtendWithDomains(MachineDictionary machine);
    public void MachineRolesAdd(object? id, IEnumerable<object> roleIds);
    public void MachineRolesRemove(object? id, IEnumerable<object> roleIds);
    public void MachinePermissionsAdd(object? id, IEnumerable<object> permissionIds);
    public void MachinePermissionsRemove(object? id, IEnumerable<object> permissionIds);
    public void MachineDomainsAdd(object? id, IEnumerable<object> domainIds);
    public void MachineDomainsRemove(object? id, IEnumerable<object> domainIds);
}
