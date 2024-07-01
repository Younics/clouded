namespace Clouded.Auth.Provider.Config;

public struct DataSourcesConfig
{
    public const string UserRoleRelationsTable = "clouded_auth_support_user_role_relations";
    public const string UserRoleRelationsPrimaryKey = "cs_user_role_rel";
    public const string UserPermissionRelationsTable =
        "clouded_auth_support_user_permission_relations";
    public const string UserPermissionRelationsPrimaryKey = "cs_user_permission_rel";

    public const string OrganizationUserRelationsTable =
        "clouded_auth_support_organization_user_relations";
    public const string OrganizationUserRelationsPrimaryKey = "cs_organization_user_rel";

    public const string RolePermissionRelationsTable =
        "clouded_auth_support_role_permission_relations";
    public const string RolePermissionRelationsPrimaryKey = "cs_role_permission_rel";

    public const string MachineRoleRelationsTable = "clouded_auth_support_machine_role_relations";
    public const string MachineRoleRelationsPrimaryKey = "cs_machine_role_rel";
    public const string MachinePermissionRelationsTable =
        "clouded_auth_support_machine_permission_relations";
    public const string MachinePermissionRelationsPrimaryKey = "cs_machine_permission_rel";

    public const string MachineDomainRelationsTable =
        "clouded_auth_support_machine_domain_relations";
    public const string MachineDomainRelationsPrimaryKey = "cs_machine_domain_rel";
}
