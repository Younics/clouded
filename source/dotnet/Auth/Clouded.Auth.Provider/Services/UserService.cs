using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.User;
using Clouded.Function.Framework.Contexts;
using Clouded.Function.Framework.Contexts.Base;
using Clouded.Function.Library;
using Clouded.Function.Library.Enums;
using Clouded.Function.Shared;
using Clouded.Results.Exceptions;
using Clouded.Shared;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class UserService(
    ApplicationOptions options,
    IUserDataSource dataSource,
    HookResolver hookResolver
)
    : EntityBaseService<IUserDataSource, UserDictionary>("User", dataSource, hookResolver),
        IUserService
{
    private readonly EntityIdentityUserOptions _entityIdentityUserOptions = options
        .Clouded
        .Auth
        .Identity
        .User;

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="organizationId"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Paginated<UserDictionary> GetAllPaginated(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        object? organizationId,
        int page = 0,
        int size = 50
    )
    {
        var users = DataSource.UsersPaginated(
            page,
            size,
            search,
            roleIds: roleIds,
            permissionIds: permissionIds,
            organizationId: organizationId
        );
        users.Items = users.Items.Select(DataSource.UserRemovePassword);

        return users;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="roleIds"></param>
    /// <param name="permissionIds"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public IEnumerable<UserDictionary> GetAll(
        object? search,
        IEnumerable<object>? roleIds,
        IEnumerable<object>? permissionIds,
        object? organizationId
    )
    {
        var users = DataSource.Users(search, roleIds, permissionIds, organizationId);
        users = users.Select(DataSource.UserRemovePassword);

        return users;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public UserDictionary GetById(object id, bool withRoles, bool withPermissions)
    {
        var user = DataSource.EntityFindById(id);
        CheckEntityFound(user);

        user = DataSource.UserRemovePassword(user);

        if (withRoles)
            DataSource.UserExtendWithRoles(user);

        if (withPermissions)
            DataSource.UserExtendWithPermissions(user);

        return user;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withRoles"></param>
    /// <param name="withPermissions"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public async Task<UserDictionary> GetByIdentity(
        object identity,
        bool withRoles,
        bool withPermissions
    )
    {
        var providerHookName = EAuthProviderFunctionType.UserRead.GetEnumName();

        if (HookResolver.IsHookEnabled(providerHookName, EFunctionType.InputHook))
        {
            var response = await HookResolver.TransformInput(
                providerHookName,
                new HookContext { Data = new { Type = "ByIdentity", Input = identity } }
            );

            // if (response.Error != null)
            //     return Problem
            //     (
            //         response.Error,
            //         statusCode: 500
            //     );

            identity = response.Data?.ToString();
        }

        if (HookResolver.IsHookEnabled(providerHookName, EFunctionType.ValidationHook))
        {
            var response = await HookResolver.Validation(
                providerHookName,
                new ValidationContext { Data = new { Type = "ByIdentity", Input = identity } }
            );

            // if (!response.Success)
            // return ValidationProblem(response.Error);
        }

        if (HookResolver.IsHookEnabled(providerHookName, EFunctionType.BeforeHook))
            await HookResolver
                .HookBefore(
                    providerHookName,
                    new HookContext { Data = new { Type = "ByIdentity", Input = identity } }
                )
                .ConfigureAwait(false);

        var user = DataSource.EntityFindByIdentity(identity);
        CheckEntityFound(user);

        user = DataSource.UserRemovePassword(user);

        if (withRoles)
            DataSource.UserExtendWithRoles(user);

        if (withPermissions)
            DataSource.UserExtendWithPermissions(user);

        if (HookResolver.IsHookEnabled(providerHookName, EFunctionType.AfterHook))
            await HookResolver
                .HookAfter(
                    providerHookName,
                    new HookContext { Data = new { Type = "ByIdentity", Output = user } }
                )
                .ConfigureAwait(false);

        if (HookResolver.IsHookEnabled(providerHookName, EFunctionType.OutputHook))
        {
            var response = await HookResolver.TransformOutput(
                providerHookName,
                new HookContext { Data = new { Type = "ByIdentity", Output = user } }
            );

            // if (response.Error != null)
            //     return Problem
            //     (
            //         response.Error,
            //         statusCode: 500
            //     );

            // return response.Data;
        }

        return user;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    public UserDictionary Create(UserDictionary data)
    {
        // User validation
        CreateRecordValidation(_entityIdentityUserOptions.ColumnIdentityArray, data);

        var user = DataSource.EntityCreate(data);
        user = DataSource.UserRemovePassword(user);

        return user;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public UserDictionary Update(object id, UserDictionary data)
    {
        var userToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(userToBeUpdated);

        // User validation
        UpdateRecordValidation(
            _entityIdentityUserOptions.ColumnIdentityArray,
            userToBeUpdated,
            data
        );

        var user = DataSource.EntityUpdate(id, data);
        user = DataSource.UserRemovePassword(user);

        return user;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddRoles(object id, UserRoleInput input)
    {
        DataSource.UserRolesAdd(id, input.RoleIds, input.OrganizationId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemoveRoles(object id, UserRoleInput input)
    {
        DataSource.UserRolesRemove(id, input.RoleIds, input.OrganizationId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddPermissions(object id, UserPermissionInput input)
    {
        DataSource.UserPermissionsAdd(id, input.PermissionIds, input.OrganizationId);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemovePermissions(object id, UserPermissionInput input)
    {
        DataSource.UserPermissionsRemove(id, input.PermissionIds, input.OrganizationId);
    }
}
