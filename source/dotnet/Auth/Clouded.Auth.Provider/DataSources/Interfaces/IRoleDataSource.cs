using Clouded.Auth.Provider.Dictionaries;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IRoleDataSource : IDataSource<RoleDictionary>
{
    public IEnumerable<PermissionDictionary> RolePermissions(object? roleId);

    public void RoleExtendWithPermissions(RoleDictionary role);

    public void RolePermissionsAdd(object? id, IEnumerable<object> permissionIds);

    public void RolePermissionsRemove(object? id, IEnumerable<object> permissionIds);
}
