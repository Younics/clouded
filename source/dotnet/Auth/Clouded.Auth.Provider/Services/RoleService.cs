using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.Role;
using Clouded.Function.Library;
using Clouded.Results.Exceptions;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class RoleService(
    ApplicationOptions options,
    IRoleDataSource dataSource,
    HookResolver hookResolver
)
    : EntityBaseService<IRoleDataSource, RoleDictionary>("Role", dataSource, hookResolver),
        IRoleService
{
    private readonly EntityIdentityRoleOptions _entityIdentityRoleOptions = options
        .Clouded
        .Auth
        .Identity
        .Role!;

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Paginated<RoleDictionary> GetAllPaginated(object? search, int page = 0, int size = 50)
    {
        var roles = DataSource.EntitiesPaginated(page, size, search);

        return roles;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public IEnumerable<RoleDictionary> GetAll(object? search)
    {
        var roles = DataSource.Entities(search);
        return roles;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withPermissions"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public RoleDictionary GetById(object id, bool withPermissions)
    {
        var role = DataSource.EntityFindById(id);
        CheckEntityFound(role);

        if (withPermissions)
            DataSource.RoleExtendWithPermissions(role);

        return role;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withPermissions"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public RoleDictionary GetByIdentity(object identity, bool withPermissions)
    {
        var role = DataSource.EntityFindByIdentity(identity);
        CheckEntityFound(role);

        if (withPermissions)
            DataSource.RoleExtendWithPermissions(role);

        return role;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    public RoleDictionary Create(RoleDictionary data)
    {
        data = data;

        // Role validation
        CreateRecordValidation(_entityIdentityRoleOptions.ColumnIdentityArray, data);

        var role = DataSource.EntityCreate(data);
        return role;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public RoleDictionary Update(object id, RoleDictionary data)
    {
        id = id!;
        data = data;

        var roleToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(roleToBeUpdated);

        // Role validation
        UpdateRecordValidation(
            _entityIdentityRoleOptions.ColumnIdentityArray,
            roleToBeUpdated,
            data
        );

        var role = DataSource.EntityUpdate(id, data);
        return role;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddPermissions(object id, RolePermissionInput input)
    {
        DataSource.RolePermissionsAdd(id, input.PermissionIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemovePermissions(object id, RolePermissionInput input)
    {
        DataSource.RolePermissionsRemove(id, input.PermissionIds);
    }
}
