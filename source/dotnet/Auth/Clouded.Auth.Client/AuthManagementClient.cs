using Clouded.Auth.Client.Base;
using Clouded.Auth.Shared;
using Clouded.Auth.Shared.Pagination;
using Clouded.Results;
using Flurl.Http;

namespace Clouded.Auth.Client;

public class AuthManagementClient(
    string apiUrl,
    string? apiKey = null,
    string? secretKey = null,
    string? cloudedKey = null
) : BaseManagementClient(apiUrl, apiKey, secretKey, cloudedKey)
{
    private const string OrganizationUrlSegment = $"v1/{RoutesConfig.OrganizationsRoutePrefix}";
    private const string UserUrlSegment = $"v1/{RoutesConfig.UsersRoutePrefix}";
    private const string RoleUrlSegment = $"v1/{RoutesConfig.RolesRoutePrefix}";
    private const string PermissionUrlSegment = $"v1/{RoutesConfig.PermissionsRoutePrefix}";
    private const string MachinesUrlSegment = $"v1/{RoutesConfig.MachinesRoutePrefix}";

    public Task<CloudedOkResult<Paginated<Dictionary<string, object?>>>> Organizations(
        int page = 0,
        int size = 50,
        CancellationToken cancellationToken = default
    ) => GetPaginatedAsync(OrganizationUrlSegment, page, size, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> Organization(
        object id,
        CancellationToken cancellationToken = default
    ) => GetByIdAsync(OrganizationUrlSegment, id, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> OrganizationCreate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(OrganizationUrlSegment, input, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> OrganizationUpdate(
        object id,
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => UpdateAsync(OrganizationUrlSegment, id, input, cancellationToken);

    public Task<bool> OrganizationDelete(
        object id,
        CancellationToken cancellationToken = default
    ) => DeleteAsync(OrganizationUrlSegment, id, cancellationToken);

    public Task<bool> OrganizationUsersAdd(
        object id,
        object[] userIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(OrganizationUrlSegment, id, "users/add", userIds, cancellationToken);

    public Task<bool> OrganizationUsersRemove(
        object id,
        object[] userIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(OrganizationUrlSegment, id, "users/remove", userIds, cancellationToken);

    public Task<CloudedOkResult<Paginated<Dictionary<string, object?>>>> Users(
        int page = 0,
        int size = 50,
        CancellationToken cancellationToken = default
    ) => GetPaginatedAsync(UserUrlSegment, page, size, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> UserById(
        object id,
        CancellationToken cancellationToken = default
    ) => GetByIdAsync(UserUrlSegment, id, cancellationToken);

    public async Task<Dictionary<string, object>> UserByIdentity(
        object identity,
        CancellationToken cancellationToken = default
    )
    {
        var request = RestClient
            .Request()
            .AppendPathSegments(UserUrlSegment, "by_identity", identity);

        request = AddCloudedKeyHeader(request);
        request = AddMachineKeyHeader(request);

        return await request.GetJsonAsync<Dictionary<string, object>>(
            HttpCompletionOption.ResponseContentRead,
            cancellationToken
        );
    }

    public Task<CloudedOkResult<Dictionary<string, object?>>> UserCreate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(UserUrlSegment, input, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> UserUpdate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(UserUrlSegment, input, cancellationToken);

    public Task<bool> UserDelete(object id, CancellationToken cancellationToken = default) =>
        DeleteAsync(UserUrlSegment, id, cancellationToken);

    public Task<bool> UserRolesAdd(
        object id,
        object[] roleIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(UserUrlSegment, id, "roles/add", roleIds, cancellationToken);

    public Task<bool> UserRolesRemove(
        object id,
        object[] roleIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(UserUrlSegment, id, "roles/remove", roleIds, cancellationToken);

    public Task<bool> UserPermissionsAdd(
        object id,
        object[] permissionIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(UserUrlSegment, id, "permissions/add", permissionIds, cancellationToken);

    public Task<bool> UserPermissionsRemove(
        object id,
        object[] permissionIds,
        CancellationToken cancellationToken = default
    ) =>
        AddOrRemoveAsync(
            UserUrlSegment,
            id,
            "permissions/remove",
            permissionIds,
            cancellationToken
        );

    public Task<CloudedOkResult<Paginated<Dictionary<string, object?>>>> Roles(
        int page = 0,
        int size = 50,
        CancellationToken cancellationToken = default
    ) => GetPaginatedAsync(RoleUrlSegment, page, size, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> Role(
        object id,
        CancellationToken cancellationToken = default
    ) => GetByIdAsync(RoleUrlSegment, id, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> RoleCreate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(RoleUrlSegment, input, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> RoleUpdate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(RoleUrlSegment, input, cancellationToken);

    public Task<bool> RoleDelete(object id, CancellationToken cancellationToken = default) =>
        DeleteAsync(RoleUrlSegment, id, cancellationToken);

    public Task<bool> RolePermissionsAdd(
        object id,
        object[] permissionIds,
        CancellationToken cancellationToken = default
    ) => AddOrRemoveAsync(RoleUrlSegment, id, "permissions/add", permissionIds, cancellationToken);

    public Task<bool> RolePermissionsRemove(
        object id,
        object[] permissionIds,
        CancellationToken cancellationToken = default
    ) =>
        AddOrRemoveAsync(
            RoleUrlSegment,
            id,
            "permissions/remove",
            permissionIds,
            cancellationToken
        );

    public Task<CloudedOkResult<Paginated<Dictionary<string, object?>>>> Permissions(
        int page = 0,
        int size = 50,
        CancellationToken cancellationToken = default
    ) => GetPaginatedAsync(PermissionUrlSegment, page, size, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> Permission(
        object id,
        CancellationToken cancellationToken = default
    ) => GetByIdAsync(PermissionUrlSegment, id, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> PermissionCreate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(PermissionUrlSegment, input, cancellationToken);

    public Task<CloudedOkResult<Dictionary<string, object?>>> PermissionUpdate(
        Dictionary<string, object?> input,
        CancellationToken cancellationToken = default
    ) => CreateAsync(PermissionUrlSegment, input, cancellationToken);

    public Task<bool> PermissionDelete(object id, CancellationToken cancellationToken = default) =>
        DeleteAsync(PermissionUrlSegment, id, cancellationToken);
}
