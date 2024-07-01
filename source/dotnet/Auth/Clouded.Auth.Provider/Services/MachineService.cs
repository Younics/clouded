using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Machine;
using Clouded.Auth.Shared.Pagination;
using Clouded.Function.Library;
using Clouded.Results.Exceptions;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class MachineService(
    ApplicationOptions options,
    IMachineDataSource dataSource,
    IDomainDataSource domainDataSource,
    IPermissionDataSource permissionDataSource,
    IRoleDataSource roleDataSource,
    HookResolver hookResolver
)
    : EntityBaseService<IMachineDataSource, MachineDictionary>("Machine", dataSource, hookResolver),
        IMachineService
{
    private readonly EntityIdentityMachineOptions _entityIdentityMachineOptions = options
        .Clouded
        .Auth
        .Identity
        .Machine!;

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Paginated<MachineDictionary> GetAllPaginated(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        int page = 0,
        int size = 50
    )
    {
        var machines = DataSource.MachinePaginated(page, size, search, roleIds, permissionIds);

        return machines;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <returns></returns>
    public IEnumerable<MachineDictionary> GetAll(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds
    )
    {
        var machines = DataSource.Machines(search, roleIds, permissionIds);

        return machines;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <param name="withDomains"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public MachineDictionary GetById(
        object id,
        bool withRoles,
        bool withPermissions,
        bool withDomains
    )
    {
        var machine = DataSource.EntityFindById(id);
        CheckEntityFound(machine);

        if (withRoles)
            DataSource.MachineExtendWithRoles(machine);

        if (withPermissions)
            DataSource.MachineExtendWithPermissions(machine);

        if (withDomains)
            DataSource.MachineExtendWithDomains(machine);

        return machine;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <param name="withDomains"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public MachineDictionary GetByIdentity(
        object identity,
        bool withRoles,
        bool withPermissions,
        bool withDomains
    )
    {
        var machine = DataSource.EntityFindByIdentity(identity);
        CheckEntityFound(machine);

        if (withRoles)
            DataSource.MachineExtendWithRoles(machine);

        if (withPermissions)
            DataSource.MachineExtendWithPermissions(machine);

        if (withDomains)
            DataSource.MachineExtendWithDomains(machine);

        return machine;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    public MachineDictionary Create(MachineDictionary data)
    {
        // Machine validation
        CreateRecordValidation(_entityIdentityMachineOptions.ColumnIdentityArray, data);

        var machine = DataSource.EntityCreate(data);
        return machine;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public MachineDictionary Update(object id, MachineDictionary data)
    {
        var machineToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(machineToBeUpdated);

        // Machine validation
        UpdateRecordValidation(
            _entityIdentityMachineOptions.ColumnIdentityArray,
            machineToBeUpdated,
            data
        );

        var machine = DataSource.EntityUpdate(id, data);
        return machine;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddRoles(object id, MachineRoleInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.RoleIds)
        {
            var relatedToBeUpdated = roleDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachineRolesAdd(id, input.RoleIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemoveRoles(object id, MachineRoleInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.RoleIds)
        {
            var relatedToBeUpdated = roleDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachineRolesRemove(id, input.RoleIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddPermissions(object id, MachinePermissionInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.PermissionIds)
        {
            var relatedToBeUpdated = permissionDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachinePermissionsAdd(id, input.PermissionIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemovePermissions(object id, MachinePermissionInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.PermissionIds)
        {
            var relatedToBeUpdated = permissionDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachinePermissionsRemove(id, input.PermissionIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddDomains(object id, MachineDomainInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.DomainIds)
        {
            var relatedToBeUpdated = domainDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachineDomainsAdd(id, input.DomainIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemoveDomains(object id, MachineDomainInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.DomainIds)
        {
            var relatedToBeUpdated = domainDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.MachineDomainsRemove(id, input.DomainIds);
    }
}
