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
using Clouded.Shared.Enums;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.DataSources;

public class OrganizationDataSource(ApplicationOptions options, IHashService hashService, IUserDataSource dataSource)
    :
        BaseDataSource<OrganizationDictionary, EntityIdentityOrganizationOptions, IdentityOrganizationMetaOptions,
            EntitySupportOptions>(options,
            hashService, options.Clouded.Auth.Identity.Organization == null
                ? []
                : [
                    options.Clouded.Auth.Identity.Organization.ColumnId,
                    options.Clouded.Auth.Identity.Organization.ColumnIdentity
                ],
            options.Clouded.Auth.Identity.Organization,
            options.Clouded.Auth.Identity.Organization?.Meta,
            options.Clouded.Auth.Identity.Organization?.Support),
        IOrganizationDataSource
{
    public IEnumerable<UserDictionary> OrganizationUsers(object? organizationId)
    {
        if (Options.Organization == null)
            return Array.Empty<UserDictionary>();

        organizationId = organizationId.Transform();

        return Context.Select<UserDictionary>
        (
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Table,
                Alias = Options.User.Table,
                SelectedColumns = new SelectColDesc[]
                {
                    new() { ColJoin = new[] { Options.User.Table, "*" }}
                },
                Join = new JoinInput[]
                {
                    new()
                    {
                        Schema = Options.Organization.Schema,
                        Table = DataSourcesConfig.OrganizationUserRelationsTable,
                        Alias = "organization_user_relation",
                        On = new ConditionColumnInput
                        {
                            Alias = Options.User.Table,
                            Column = Options.User.ColumnId,
                            TargetAlias = "organization_user_relation",
                            TargetColumn = "user_id",
                            Operator = EOperator.Equals
                        },
                    }
                },
                Where = new ConditionValueInput
                {
                    Alias = "organization_user_relation",
                    Column = "organization_id",
                    Value = organizationId,
                    Operator = EOperator.Equals
                }
            }
        );
    }

    public void OrganizationExtendWithUsers(OrganizationDictionary organization)
    {
        if (Options.Organization == null)
            return;

        organization = organization.Transform();

        var users = OrganizationUsers(organization[Options.Organization.ColumnId])
            .Select(dataSource.UserRemovePassword);

        organization.Add("clouded_users", users);
    }

    public override void EntityMetaDataUpdate(object? organizationId, Dictionary<string, object?> metaData)
    {
        if (Options.Organization == null || EntityMetaOptions == null)
            throw new NotSupportedException();

        organizationId = organizationId.Transform();
        metaData = metaData.Transform();

        Context.Delete
        (
            new DeleteInput
            {
                Schema = Options.Organization.Schema,
                Table = EntityMetaOptions.Table,
                Alias = EntityMetaOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntityMetaOptions.Table,
                    Column = EntityMetaOptions.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = organizationId
                }
            }
        );

        foreach (var meta in metaData)
        {
            Context.Create<OrganizationDictionary>
            (
                new CreateInput
                {
                    Schema = Options.Organization.Schema,
                    Table = EntityMetaOptions.Table,
                    Data = new OrganizationDictionary
                    {
                        { EntityMetaOptions.ColumnKey, meta.Key },
                        { EntityMetaOptions.ColumnValue, meta.Value },
                        { EntityMetaOptions.ColumnRelatedEntityId, organizationId }
                    }
                }
            );
        }
    }

    public void OrganizationUsersAdd(object? id, IEnumerable<object> userIds)
    {
        if (Options.Organization == null)
            throw new NotSupportedException();

        id = id.Transform();
        var userIdsTransformed = userIds.Transform();

        foreach (var userId in userIdsTransformed)
        {
            Context.Create<OrganizationDictionary>
            (
                new CreateInput
                {
                    Schema = Options.Organization.Schema,
                    Table = DataSourcesConfig.OrganizationUserRelationsTable,
                    Data = new OrganizationDictionary { { "organization_id", id }, { "user_id", userId } }
                }
            );
        }
    }

    public void OrganizationUsersRemove(object? id, IEnumerable<object> userIds)
    {
        if (Options.Organization == null)
            throw new NotSupportedException();

        id = id.Transform();
        var userIdsTransformed = userIds.Transform();

        if (Options.Role != null)
        {
            Context.Delete
            (
                new DeleteInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserRoleRelationsTable,
                    Alias = "user_role_relation",
                    Where = new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            new ConditionValueInput
                            {
                                Alias = "user_role_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = id
                            },
                            new ConditionValueInput
                            {
                                Alias = "user_role_relation", Column = "user_id", Operator = EOperator.In, Value = userIdsTransformed
                            }
                        }
                    }
                }
            );
        }

        if (Options.Permission != null)
        {
            Context.Delete
            (
                new DeleteInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserPermissionRelationsTable,
                    Alias = "user_permission_relation",
                    Where = new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            new ConditionValueInput
                            {
                                Alias = "user_permission_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = id
                            },
                            new ConditionValueInput
                            {
                                Alias = "user_permission_relation",
                                Column = "user_id",
                                Operator = EOperator.In,
                                Value = userIdsTransformed
                            }
                        }
                    }
                }
            );
        }

        Context.Delete
        (
            new DeleteInput
            {
                Schema = Options.Organization.Schema,
                Table = DataSourcesConfig.OrganizationUserRelationsTable,
                Alias = "organization_user",
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = "organization_user", Column = "organization_id", Operator = EOperator.Equals, Value = id
                        },
                        new ConditionValueInput
                        {
                            Alias = "organization_user", Column = "user_id", Operator = EOperator.In, Value = userIdsTransformed
                        }
                    }
                }
            }
        );
    }

    public override void SupportTableSetup()
    {
        if (Options.Organization == null)
            return;

        var organizationColumnId = GetColumnId
        (
            Options.Organization.Schema,
            Options.Organization.Table,
            Options.Organization.ColumnId
        );
        var userColumnId = GetColumnId
        (
            Options.User.Schema,
            Options.User.Table,
            Options.User.ColumnId
        );

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Organization.Schema,
                Name = DataSourcesConfig.OrganizationUserRelationsTable,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = "organization_id",
                        TypeRaw = organizationColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "organization",
                            TargetSchema = Options.Organization.Schema,
                            TargetTable = Options.Organization.Table,
                            TargetColumn = Options.Organization.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    },
                    new ColumnInput
                    {
                        Name = "user_id",
                        TypeRaw = userColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "user",
                            TargetSchema = Options.User.Schema,
                            TargetTable = Options.User.Table,
                            TargetColumn = Options.User.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    }
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{DataSourcesConfig.OrganizationUserRelationsPrimaryKey}__user_organization_id",
                        Columns = new[] { "organization_id", "user_id" }
                    }
                }
            },
            true
        );

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Organization.Schema,
                Name = EntityMetaOptions!.Table,
                Columns = new[]
                {
                    new ColumnInput { Name = EntityMetaOptions.ColumnKey, Type = EColumnType.Varchar, NotNull = true },
                    new ColumnInput { Name = EntityMetaOptions.ColumnValue, Type = EColumnType.Text, NotNull = true }, new ColumnInput
                    {
                        Name = EntityMetaOptions.ColumnRelatedEntityId,
                        TypeRaw = organizationColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "organization",
                            TargetSchema = Options.Organization.Schema,
                            TargetTable = Options.Organization.Table,
                            TargetColumn = Options.Organization.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    }
                }
            },
            true
        );
    }
}
