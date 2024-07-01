using Clouded.Auth.Provider.Config;
using Clouded.Auth.Provider.DataSources.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.DataSources;

public class RoleDataSource(ApplicationOptions options, IHashService hashService)
    : BaseDataSource<RoleDictionary, EntityIdentityRoleOptions, EntityMetaOptions, EntitySupportOptions>(options,
            hashService,
            options.Clouded.Auth.Identity.Role == null
                ? []
                : [
                    options.Clouded.Auth.Identity.Role.ColumnId, options.Clouded.Auth.Identity.Role.ColumnIdentity
                ],
            options.Clouded.Auth.Identity.Role, options.Clouded.Auth.Identity.Role?.Meta,
            options.Clouded.Auth.Identity.Role?.Support),
        IRoleDataSource
{
    public IEnumerable<PermissionDictionary> RolePermissions(object? roleId)
    {
        if (Options.Role == null || Options.Permission == null)
            return Array.Empty<PermissionDictionary>();

        roleId = roleId.Transform();

        return Context.Select<PermissionDictionary>
        (
            new SelectInput
            {
                Schema = Options.Permission.Schema,
                Table = Options.Permission.Table,
                Alias = "permission",
                Join = new JoinInput[]
                {
                    new()
                    {
                        Schema = Options.Role.Schema,
                        Table = DataSourcesConfig.RolePermissionRelationsTable,
                        Alias = "role_permission_relation",
                        On = new ConditionColumnInput
                        {
                            Alias = "permission",
                            Column = Options.Permission.ColumnId,
                            TargetAlias = "role_permission_relation",
                            TargetColumn = "permission_id",
                            Operator = EOperator.Equals
                        },
                    }
                },
                Where = new ConditionValueInput
                {
                    Alias = "role_permission_relation",
                    Column = "role_id",
                    Value = roleId,
                    Operator = EOperator.Equals
                }
            }
        );
    }

    public void RoleExtendWithPermissions(RoleDictionary role)
    {
        if (Options.Role == null || Options.Permission == null || !role.ContainsKey(Options.Role.ColumnId))
            return;

        role = role.Transform();

        role.Add("clouded_permissions", RolePermissions(role[Options.Role.ColumnId]));
    }

    public void RolePermissionsAdd(object? id, IEnumerable<object> permissionIds)
    {
        if (Options.Role == null || Options.Permission == null)
            throw new NotSupportedException();

        id = id.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        foreach (var permissionId in permissionIdsTransformed)
        {
            Context.Create
            (
                new CreateInput
                {
                    Schema = Options.Role.Schema,
                    Table = DataSourcesConfig.RolePermissionRelationsTable,
                    Data = new RoleDictionary { { "role_id", id }, { "permission_id", permissionId } }
                }
            );
        }
    }

    public void RolePermissionsRemove(object? id, IEnumerable<object> permissionIds)
    {
        if (Options.Role == null || Options.Permission == null)
            throw new NotSupportedException();

        id = id.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        Context.Delete
        (
            new DeleteInput
            {
                Schema = Options.Role.Schema,
                Table = DataSourcesConfig.RolePermissionRelationsTable,
                Alias = "role_permission",
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = "role_permission", Column = "role_id", Operator = EOperator.Equals, Value = id
                        },
                        new ConditionValueInput
                        {
                            Alias = "role_permission",
                            Column = "permission_id",
                            Operator = EOperator.In,
                            Value = permissionIdsTransformed
                        }
                    }
                }
            }
        );
    }

    public override void SupportTableSetup()
    {
        if (Options.Role == null || Options.Permission == null)
            return;

        var roleColumnId = GetColumnId
        (
            Options.Role.Schema,
            Options.Role.Table,
            Options.Role.ColumnId
        );
        var permissionColumnId = GetColumnId
        (
            Options.Permission.Schema,
            Options.Permission.Table,
            Options.Permission.ColumnId
        );

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Role.Schema,
                Name = DataSourcesConfig.RolePermissionRelationsTable,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = "role_id",
                        TypeRaw = roleColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "role",
                            TargetSchema = Options.Role.Schema,
                            TargetTable = Options.Role.Table,
                            TargetColumn = Options.Role.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    },
                    new ColumnInput
                    {
                        Name = "permission_id",
                        TypeRaw = permissionColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "permission",
                            TargetSchema = Options.Permission.Schema,
                            TargetTable = Options.Permission.Table,
                            TargetColumn = Options.Permission.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    }
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{DataSourcesConfig.RolePermissionRelationsPrimaryKey}__role_permission_id",
                        Columns = new[] { "role_id", "permission_id" }
                    }
                }
            },
            true
        );
    }
}
