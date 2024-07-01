using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Machine;
using Clouded.Auth.Shared.Pagination;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IMachineService
{
    public Paginated<MachineDictionary> GetAllPaginated(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        int page = 0,
        int size = 50
    );

    public IEnumerable<MachineDictionary> GetAll(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds
    );

    public MachineDictionary GetById(
        object id,
        bool withRoles,
        bool withPermissions,
        bool withDomains
    );

    public MachineDictionary GetByIdentity(
        object identity,
        bool withRoles,
        bool withPermissions,
        bool withDomains
    );

    public MachineDictionary Create(MachineDictionary data);

    public MachineDictionary Update(object id, MachineDictionary data);

    public void DeleteById(object id);

    public void AddRoles(object id, MachineRoleInput input);

    public void RemoveRoles(object id, MachineRoleInput input);

    public void AddPermissions(object id, MachinePermissionInput input);

    public void RemovePermissions(object id, MachinePermissionInput input);

    public void AddDomains(object id, MachineDomainInput input);

    public void RemoveDomains(object id, MachineDomainInput input);
}
