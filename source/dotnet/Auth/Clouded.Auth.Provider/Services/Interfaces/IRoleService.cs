using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.Role;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IRoleService
{
    public Paginated<RoleDictionary> GetAllPaginated(object? search, int page = 0, int size = 50);

    public IEnumerable<RoleDictionary> GetAll(object? search);

    public RoleDictionary GetById(object id, bool withPermissions);

    public RoleDictionary GetByIdentity(object identity, bool withPermissions);

    public RoleDictionary Create(RoleDictionary data);

    public RoleDictionary Update(object id, RoleDictionary data);

    public void DeleteById(object id);

    public void AddPermissions(object id, RolePermissionInput input);

    public void RemovePermissions(object id, RolePermissionInput input);
}
