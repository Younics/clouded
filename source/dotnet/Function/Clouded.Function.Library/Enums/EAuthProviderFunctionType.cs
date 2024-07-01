namespace Clouded.Function.Library.Enums;

public enum EAuthProviderFunctionType
{
    OrganizationCreate,
    OrganizationRead,
    OrganizationUpdate,
    OrganizationDelete,
    OrganizationAddUsers,
    OrganizationRemoveUsers,

    UserCreate,
    UserRead,
    UserUpdate,
    UserDelete,
    UserAddRoles,
    UserRemoveRoles,
    UserAddPermissions,
    UserRemovePermissions,

    RoleCreate,
    RoleRead,
    RoleUpdate,
    RoleDelete,
    RoleAddPermissions,
    RoleRemovePermissions,

    PermissionCreate,
    PermissionRead,
    PermissionUpdate,
    PermissionDelete,

    TokenGenerate,
    TokenRefresh,

    PasswordForgot,
    PasswordReset
}
