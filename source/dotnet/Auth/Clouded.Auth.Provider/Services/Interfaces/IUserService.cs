using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.User;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IUserService
{
    public Paginated<UserDictionary> GetAllPaginated(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        object? organizationId,
        int page = 0,
        int size = 50
    );

    public IEnumerable<UserDictionary> GetAll(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        object? organizationId
    );

    public UserDictionary GetById(object id, bool withRoles = false, bool withPermissions = false);

    public Task<UserDictionary> GetByIdentity(
        object identity,
        bool withRoles = false,
        bool withPermissions = false
    );

    public UserDictionary Create(UserDictionary data);

    public UserDictionary Update(object id, UserDictionary data);

    public void DeleteById(object id);

    public void AddRoles(object id, UserRoleInput input);

    public void RemoveRoles(object id, UserRoleInput input);

    public void AddPermissions(object id, UserPermissionInput input);

    public void RemovePermissions(object id, UserPermissionInput input);
}
