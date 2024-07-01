using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Core.DataSource.Shared;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IUserDataSource : IDataSource<UserDictionary>
{
    public IEnumerable<ColumnResult> GetEntityColumns(bool withoutPassword = false);

    public Paginated<UserDictionary> UsersPaginated(
        int page,
        int size,
        object? search = null,
        (IEnumerable<ColumnResult>, Dictionary<string, object?>)? filter = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        object? organizationId = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );

    public IEnumerable<UserDictionary> Users(
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        object? organizationId = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );

    public UserDictionary UserRemovePassword(UserDictionary user);

    public IEnumerable<OrganizationDictionary> UserOrganizations(object? userId);

    public IEnumerable<RoleDictionary> UserRoles(object? userId, object? organizationId = null);

    public IEnumerable<PermissionDictionary> UserPermissions(
        object? userId,
        object? organizationId = null
    );
    public IEnumerable<PermissionDictionary> UserPermissionsWithRolePermissions(object? userId);
    public void UserExtendWithRoles(UserDictionary user);
    public void UserExtendWithPermissions(UserDictionary user);
    public UserDictionary UserSupportTableByPasswordResetToken(string resetToken);
    public void StoreUserFacebookCode(string code, object userId);
    public void StoreUserGoogleCode(string code, object userId);
    public void StoreUserAppleCode(string code, object userId);
    public void StoreAppleData(string code, AppleUser userData);
    public DataSourceDictionary GetAppleData(string code);
    public string? GeneratePasswordResetToken(object? userId, bool reset = false);
    public void UserRolesAdd(
        object? id,
        IEnumerable<object> roleIds,
        object? organizationId = null
    );
    public void UserRolesRemove(
        object? id,
        IEnumerable<object> roleIds,
        object? organizationId = null
    );
    public void UserPermissionsAdd(
        object? id,
        IEnumerable<object> permissionIds,
        object? organizationId = null
    );

    public void UserPermissionsRemove(
        object? id,
        IEnumerable<object> permissionIds,
        object? organizationId = null
    );

    public UserDictionary RefreshTokenFindValid(string token);

    public UserDictionary RefreshTokenCreate(object? userId, string token, DateTime expires);

    public void RefreshTokenDelete(string token);

    public void RefreshTokensDelete(object? userId);

    public void RefreshTokenDeleteExpired();
}
