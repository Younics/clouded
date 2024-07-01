using Clouded.Auth.Provider.Config;
using Clouded.Auth.Provider.DataSources.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Pagination;
using Clouded.Auth.Shared.Token.Input;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;
using Clouded.Shared;
using Clouded.Shared.Enums;

namespace Clouded.Auth.Provider.DataSources;

public class UserDataSource(ApplicationOptions options, IHashService hashService)
    : BaseDataSource<
        UserDictionary,
        EntityIdentityUserOptions,
        IdentityUserMetaOptions,
        IdentityUserSupportOptions
    >(
        options,
        hashService,
        new[]
        {
            options.Clouded.Auth.Identity.User.ColumnId,
            options.Clouded.Auth.Identity.User.ColumnPassword
        }.Concat(options.Clouded.Auth.Identity.User.ColumnIdentityArray),
        options.Clouded.Auth.Identity.User,
        options.Clouded.Auth.Identity.User.Meta,
        options.Clouded.Auth.Identity.User.Support
    ),
        IUserDataSource
{
    public IEnumerable<ColumnResult> GetEntityColumns(bool withoutPassword = false)
    {
        if (withoutPassword)
        {
            return GetEntityColumns()
                .Where(column => column.Name != EntityIdentityOptions?.ColumnPassword);
        }

        return base.GetEntityColumns();
    }

    public override UserDictionary EntityCreate(
        UserDictionary data,
        DataSourceDictionary? supportData = null
    )
    {
        var columns = GetEntityColumns();

        data = data.Transform(columns);
        supportData = supportData?.Transform() ?? new DataSourceDictionary();

        var salt = Generator.RandomString(64);
        var password = (string?)data[Options.User.ColumnPassword];

        // TODO: Password not null

        // Hash password
        data[Options.User.ColumnPassword] = HashService.Hash(password, salt);

        var user = Context.Create<UserDictionary>(
            new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Table,
                ReturnColumns = new[] { "*" },
                Data = data
            }
        );

        supportData[Options.User.Support.ColumnBlocked] = false;
        supportData[Options.User.Support.ColumnAdminAccess] = false;
        supportData[Options.User.Support.ColumnSalt] = salt;
        supportData[Options.User.Support.ColumnRelatedEntityId] = user[Options.User.ColumnId];

        Context.Create(
            new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = supportData
            }
        );

        return user;
    }

    public override UserDictionary EntityUpdate(
        object? id,
        UserDictionary data,
        DataSourceDictionary? supportData = null
    )
    {
        var columns = GetEntityColumns();

        id = id.Transform();
        data = data.Transform(columns);
        supportData = supportData?.Transform() ?? EntitySupportTable(id);

        string salt;

        if (supportData.Any())
        {
            salt = (string)supportData[Options.User.Support.ColumnSalt]!;

            Context.Update<UserDictionary>(
                new UpdateInput
                {
                    Schema = Options.User.Schema,
                    Table = Options.User.Support.Table,
                    Alias = Options.User.Support.Table,
                    Data = supportData,
                    Where = new ConditionValueInput
                    {
                        Alias = Options.User.Support.Table,
                        Column = Options.User.Support.ColumnRelatedEntityId,
                        Operator = EOperator.Equals,
                        Value = id
                    }
                }
            );
        }
        else
        {
            salt = Generator.RandomString(64);
            supportData[Options.User.Support.ColumnBlocked] = false;
            supportData[Options.User.Support.ColumnAdminAccess] = false;
            supportData[Options.User.Support.ColumnSalt] = salt;
            supportData[Options.User.Support.ColumnRelatedEntityId] = id;

            Context.Create(
                new CreateInput
                {
                    Schema = Options.User.Schema,
                    Table = Options.User.Support.Table,
                    Data = supportData
                }
            );
        }

        // Hash password
        if (data.ContainsKey(Options.User.ColumnPassword))
        {
            // TODO: Password not null

            data[Options.User.ColumnPassword] = HashService.Hash((string)data.Password, salt);
        }

        var user = Context.Update<UserDictionary>(
            new UpdateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Table,
                Alias = Options.User.Table,
                ReturnColumns = new[] { "*" },
                Data = data,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Table,
                    Column = Options.User.ColumnId,
                    Operator = EOperator.Equals,
                    Value = id
                }
            }
        );

        return user;
    }

    public IEnumerable<UserDictionary> Users(
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        object? organizationId = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    )
    {
        search = search.Transform();
        organizationId = organizationId.Transform();
        var roleIdsTransformed = roleIds?.Transform();
        var permissionIdsTransformed = permissionIds?.Transform();

        var join = new List<JoinInput>();
        var where = new List<ICondition>();

        if (Options.Organization != null && organizationId != null)
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.OrganizationUserRelationsTable,
                    Alias = "organization_user_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "organization_user_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add(
                new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = "organization_user_relation",
                            Column = "organization_id",
                            Operator = EOperator.Equals,
                            Value = organizationId
                        }
                    }
                }
            );
        }

        if (Options.Role != null && roleIdsTransformed is { Count: > 0 })
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserRoleRelationsTable,
                    Alias = "user_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_role_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            var roleCondition = new ConditionValueInput
            {
                Alias = "user_role_relation",
                Column = "role_id",
                Operator = EOperator.In,
                Value = roleIdsTransformed
            };

            if (Options.Organization != null && organizationId != null)
            {
                where.Add(
                    new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            roleCondition,
                            new ConditionValueInput
                            {
                                Alias = "user_role_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = organizationId
                            }
                        }
                    }
                );
            }
            else
            {
                where.Add(roleCondition);
            }
        }

        if (Options.Permission != null && permissionIdsTransformed is { Count: > 0 })
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserPermissionRelationsTable,
                    Alias = "user_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_permission_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            var permissionCondition = new ConditionValueInput
            {
                Alias = "user_permission_relation",
                Column = "permission_id",
                Operator = EOperator.In,
                Value = permissionIdsTransformed
            };

            if (Options.Organization != null && organizationId != null)
            {
                where.Add(
                    new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            permissionCondition,
                            new ConditionValueInput
                            {
                                Alias = "user_permission_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = organizationId
                            }
                        }
                    }
                );
            }
            else
            {
                where.Add(permissionCondition);
            }
        }

        if (search != null)
        {
            where.Add(
                new ConditionOrInput
                {
                    Conditions = Options.User.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = Options.User.Table,
                                Column = column,
                                Operator = EOperator.Contains,
                                Mode = EMode.Insensitive,
                                Value = search
                            }
                    )
                }
            );
        }

        return GetAll(
            Options.User.Schema,
            Options.User.Table,
            Options.User.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { Options.User.Table, "*" } } },
            join.Any() ? join : null,
            where.Any() ? new ConditionAndInput { Conditions = where } : null,
            new[]
            {
                new OrderInput
                {
                    Alias = Options.User.Table,
                    Column = orderByColumn ?? Options.User.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            true
        );
    }

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
    )
    {
        search = search.Transform();
        organizationId = organizationId.Transform();
        var roleIdsTransformed = roleIds?.Transform();
        var permissionIdsTransformed = permissionIds?.Transform();

        var join = new List<JoinInput>();
        var where = new List<ICondition>
        {
            new ConditionAndInput
            {
                Conditions = new ICondition[]
                {
                    filter == null
                        ? new DefaultConditionInput()
                        : new ConditionAndInput
                        {
                            Conditions = filter.Value.Item1
                                .Where(i => filter.Value.Item2[i.GetKey()] != null)
                                .Select(
                                    (col) =>
                                        col.BuildFilterCondition(
                                            Options.User.Table,
                                            filter.Value.Item2[col.GetKey()]!
                                        )
                                )
                                .ToList()
                        }
                }
            }
        };

        if (Options.Organization != null && organizationId != null)
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.OrganizationUserRelationsTable,
                    Alias = "organization_user_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "organization_user_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add(
                new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = "organization_user_relation",
                            Column = "organization_id",
                            Operator = EOperator.Equals,
                            Value = organizationId
                        }
                    }
                }
            );
        }

        if (Options.Role != null && roleIdsTransformed is { Count: > 0 })
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserRoleRelationsTable,
                    Alias = "user_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_role_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            var roleCondition = new ConditionValueInput
            {
                Alias = "user_role_relation",
                Column = "role_id",
                Operator = EOperator.In,
                Value = roleIdsTransformed
            };

            if (Options.Organization != null && organizationId != null)
            {
                where.Add(
                    new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            roleCondition,
                            new ConditionValueInput
                            {
                                Alias = "user_role_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = organizationId
                            }
                        }
                    }
                );
            }
            else
            {
                where.Add(roleCondition);
            }
        }

        if (Options.Permission != null && permissionIdsTransformed is { Count: > 0 })
        {
            join.Add(
                new JoinInput
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserPermissionRelationsTable,
                    Alias = "user_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_permission_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            var permissionCondition = new ConditionValueInput
            {
                Alias = "user_permission_relation",
                Column = "permission_id",
                Operator = EOperator.In,
                Value = permissionIdsTransformed
            };

            if (Options.Organization != null && organizationId != null)
            {
                where.Add(
                    new ConditionAndInput
                    {
                        Conditions = new[]
                        {
                            permissionCondition,
                            new ConditionValueInput
                            {
                                Alias = "user_permission_relation",
                                Column = "organization_id",
                                Operator = EOperator.Equals,
                                Value = organizationId
                            }
                        }
                    }
                );
            }
            else
            {
                where.Add(permissionCondition);
            }
        }

        if (search != null)
        {
            where.Add(
                new ConditionOrInput
                {
                    Conditions = Options.User.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = Options.User.Table,
                                Column = column,
                                Operator = EOperator.Contains,
                                Mode = EMode.Insensitive,
                                Value = search
                            }
                    )
                }
            );
        }

        return GetPaginated(
            Options.User.Schema,
            Options.User.Table,
            Options.User.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { Options.User.Table, "*" } } },
            page,
            size,
            join,
            where.Any() ? new ConditionAndInput { Conditions = where } : null,
            new[]
            {
                new OrderInput
                {
                    Alias = Options.User.Table,
                    Column = orderByColumn ?? Options.User.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            distinct: true
        );
    }

    public IEnumerable<OrganizationDictionary> UserOrganizations(object? userId)
    {
        if (Options.Organization == null)
            return Array.Empty<OrganizationDictionary>();

        userId = userId.Transform();

        var organizations = Context.Select<OrganizationDictionary>(
            new SelectInput
            {
                Schema = Options.Organization.Schema,
                Table = Options.Organization.Table,
                Alias = Options.Organization.Table,
                SelectedColumns = new SelectColDesc[]
                {
                    new() { ColJoin = new[] { Options.Organization.Table, "*" } }
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
                            Alias = Options.Organization.Table,
                            Column = Options.Organization.ColumnId,
                            TargetAlias = "organization_user_relation",
                            TargetColumn = "organization_id",
                            Operator = EOperator.Equals
                        },
                    }
                },
                Where = new ConditionValueInput
                {
                    Alias = "organization_user_relation",
                    Column = "user_id",
                    Value = userId,
                    Operator = EOperator.Equals
                }
            }
        );

        return organizations;
    }

    public IEnumerable<RoleDictionary> UserRoles(object? userId, object? organizationId = null)
    {
        if (Options.Role == null)
            return Array.Empty<RoleDictionary>();

        userId = userId.Transform();
        organizationId = organizationId.Transform();

        var whereUserIdInput = new ConditionValueInput
        {
            Alias = "user_role_relation",
            Column = "user_id",
            Value = userId,
            Operator = EOperator.Equals
        };

        var selectedColumns = new List<SelectColDesc>
        {
            new() { ColJoin = new[] { Options.Role.Table, "*" } }
        };

        var input = new SelectInput
        {
            Schema = Options.Role.Schema,
            Table = Options.Role.Table,
            Alias = Options.Role.Table,
            SelectedColumns = selectedColumns,
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserRoleRelationsTable,
                    Alias = "user_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Role.Table,
                        Column = Options.Role.ColumnId,
                        TargetAlias = "user_role_relation",
                        TargetColumn = "role_id",
                        Operator = EOperator.Equals
                    }
                }
            },
            Where = whereUserIdInput
        };

        if (Options.Organization != null)
        {
            selectedColumns.Add(
                new SelectColDesc { ColJoin = new[] { "user_role_relation", "organization_id" } }
            );

            if (organizationId != null)
            {
                input.Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        whereUserIdInput,
                        new ConditionValueInput
                        {
                            Alias = "user_role_relation",
                            Column = "organization_id",
                            Value = organizationId,
                            Operator = EOperator.Equals
                        }
                    }
                };
            }
        }

        var roles = Context.Select<RoleDictionary>(input);
        return roles;
    }

    public IEnumerable<PermissionDictionary> UserPermissions(
        object? userId,
        object? organizationId = null
    )
    {
        if (Options.Permission == null)
            return Array.Empty<PermissionDictionary>();

        userId = userId.Transform();
        organizationId = organizationId.Transform();

        var whereUserIdInput = new ConditionValueInput
        {
            Alias = DataSourcesConfig.UserPermissionRelationsTable,
            Column = "user_id",
            Value = userId,
            Operator = EOperator.Equals
        };

        var selectedColumns = new List<SelectColDesc>
        {
            new() { ColJoin = new[] { Options.Permission.Table, "*" } }
        };

        var input = new SelectInput
        {
            Schema = Options.Permission.Schema,
            Table = Options.Permission.Table,
            Alias = Options.Permission.Table,
            SelectedColumns = selectedColumns,
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserPermissionRelationsTable,
                    Alias = DataSourcesConfig.UserPermissionRelationsTable,
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Permission.Table,
                        Column = Options.Permission.ColumnId,
                        TargetAlias = DataSourcesConfig.UserPermissionRelationsTable,
                        TargetColumn = "permission_id",
                        Operator = EOperator.Equals
                    },
                }
            },
            Where = whereUserIdInput
        };

        if (Options.Organization != null)
        {
            selectedColumns.Add(
                new SelectColDesc()
                {
                    ColJoin = new[]
                    {
                        DataSourcesConfig.UserPermissionRelationsTable,
                        "organization_id"
                    }
                }
            );

            if (organizationId != null)
            {
                input.Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        whereUserIdInput,
                        new ConditionValueInput
                        {
                            Alias = DataSourcesConfig.UserPermissionRelationsTable,
                            Column = "organization_id",
                            Value = organizationId,
                            Operator = EOperator.Equals
                        }
                    }
                };
            }
        }

        var permissions = Context.Select<PermissionDictionary>(input);
        return permissions;
    }

    public IEnumerable<PermissionDictionary> UserPermissionsWithRolePermissions(object? userId)
    {
        userId = userId.Transform();

        if (Options.Role == null)
            return UserPermissions(userId);

        if (Options.Permission == null)
            throw new NotSupportedException();

        var input = new SelectInput
        {
            Schema = Options.User.Schema,
            Table = Options.User.Table,
            Alias = Options.User.Table,
            Distinct = true,
            SelectedColumns = new SelectColDesc[]
            {
                new() { ColJoin = new[] { Options.Permission.Table, Options.Permission.ColumnId } },
                new()
                {
                    ColJoin = new[] { Options.Permission.Table, Options.Permission.ColumnIdentity }
                }
            },
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserRoleRelationsTable,
                    Alias = "user_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_role_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                },
                new()
                {
                    Schema = Options.User.Schema,
                    Table = DataSourcesConfig.UserPermissionRelationsTable,
                    Alias = "user_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.User.Table,
                        Column = Options.User.ColumnId,
                        TargetAlias = "user_permission_relation",
                        TargetColumn = "user_id",
                        Operator = EOperator.Equals
                    }
                },
                new()
                {
                    Schema = Options.Role.Schema,
                    Table = DataSourcesConfig.RolePermissionRelationsTable,
                    Alias = "role_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = "user_role_relation",
                        Column = "role_id",
                        TargetAlias = "role_permission_relation",
                        TargetColumn = "role_id",
                        Operator = EOperator.Equals
                    }
                },
                new()
                {
                    Schema = Options.Permission.Schema,
                    Table = Options.Permission.Table,
                    Alias = Options.Permission.Table,
                    On = new ConditionOrInput
                    {
                        Conditions = new[]
                        {
                            new ConditionColumnInput
                            {
                                Alias = Options.Permission.Table,
                                Column = "id",
                                TargetAlias = "user_permission_relation",
                                TargetColumn = "permission_id",
                                Operator = EOperator.Equals
                            },
                            new ConditionColumnInput
                            {
                                Alias = Options.Permission.Table,
                                Column = "id",
                                TargetAlias = "role_permission_relation",
                                TargetColumn = "permission_id",
                                Operator = EOperator.Equals
                            }
                        }
                    }
                }
            },
            Where = new ConditionValueInput
            {
                Alias = Options.User.Table,
                Column = Options.User.ColumnId,
                Value = userId,
                Operator = EOperator.Equals
            }
        };

        var permissions = Context
            .Select<PermissionDictionary>(input)
            .Where(permission => permission[Options.Permission.ColumnIdentity] != null);

        return permissions;
    }

    public UserDictionary UserRemovePassword(UserDictionary user)
    {
        user = user.Transform();

        user.Remove(Options.User.ColumnPassword);
        return user;
    }

    public void UserExtendWithRoles(UserDictionary user)
    {
        if (Options.Role == null || !user.ContainsKey(Options.User.ColumnId))
            return;

        user = user.Transform();

        user.Add("clouded_roles", UserRoles(user[Options.User.ColumnId]));
    }

    public void UserExtendWithPermissions(UserDictionary user)
    {
        if (Options.Permission == null || !user.ContainsKey(Options.User.ColumnId))
            return;

        user = user.Transform();

        user.Add("clouded_permissions", UserPermissions(user[Options.User.ColumnId]));
    }

    public UserDictionary UserSupportTableByPasswordResetToken(string resetToken)
    {
        var user = Context.SelectSingle<UserDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Alias = Options.User.Support.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnPasswordResetToken,
                    Operator = EOperator.Equals,
                    Value = resetToken
                }
            }
        );

        return user;
    }

    public void StoreUserFacebookCode(string code, object userId)
    {
        var userSupportData = Context.SelectSingle<UserDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Alias = Options.User.Support.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            }
        );

        if (userSupportData.Any())
        {
            var input = new UpdateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnFbAccessCode, code }
                },
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            };

            Context.Update(input);
        }
        else
        {
            var input = new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnRelatedEntityId, userId },
                    { Options.User.Support.ColumnFbAccessCode, code }
                }
            };

            Context.Create(input);
        }
    }

    public void StoreUserGoogleCode(string code, object userId)
    {
        var userSupportData = Context.SelectSingle<UserDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Alias = Options.User.Support.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            }
        );

        if (userSupportData.Any())
        {
            var input = new UpdateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnGoogleAccessCode, code }
                },
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            };

            Context.Update(input);
        }
        else
        {
            var input = new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnRelatedEntityId, userId },
                    { Options.User.Support.ColumnGoogleAccessCode, code }
                }
            };

            Context.Create(input);
        }
    }

    public DataSourceDictionary GetAppleData(string code)
    {
        return Context.SelectSingle<DataSourceDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.AppleData.Table,
                Alias = Options.User.AppleData.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.AppleData.Table,
                    Column = Options.User.AppleData.ColumnCode,
                    Operator = EOperator.Equals,
                    Value = code
                }
            }
        );
    }

    public void StoreAppleData(string code, AppleUser userData)
    {
        Context.Create<DataSourceDictionary>(
            new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.AppleData.Table,
                ReturnColumns = new[] { "*" },
                Data = new DataSourceDictionary
                {
                    { Options.User.AppleData.ColumnCode, code.Transform() },
                    { Options.User.AppleData.ColumnEmail, userData.Email.Transform() },
                    {
                        Options.User.AppleData.ColumnFirstName,
                        userData.Name?.FirstName.Transform()
                    },
                    { Options.User.AppleData.ColumnLastName, userData.Name?.LastName.Transform() }
                }
            }
        );
    }

    public void StoreUserAppleCode(string code, object userId)
    {
        var userSupportData = Context.SelectSingle<UserDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Alias = Options.User.Support.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            }
        );

        if (userSupportData.Any())
        {
            var input = new UpdateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnAppleAccessCode, code }
                },
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                }
            };

            Context.Update(input);
        }
        else
        {
            var input = new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.Support.ColumnRelatedEntityId, userId },
                    { Options.User.Support.ColumnAppleAccessCode, code }
                }
            };

            Context.Create(input);
        }
    }

    public string? GeneratePasswordResetToken(object? userId, bool reset = false)
    {
        userId = userId.Transform();

        string? passwordResetToken = null;

        if (!reset)
            passwordResetToken = Generator.RandomString(128, true, false);

        Context.Update(
            new UpdateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.Support.Table,
                Alias = Options.User.Support.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.Support.Table,
                    Column = Options.User.Support.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = userId
                },
                Data = new DataSourceDictionary
                {
                    [Options.User.Support.ColumnPasswordResetToken] = passwordResetToken
                }
            }
        );

        return passwordResetToken;
    }

    public void UserRolesAdd(object? id, IEnumerable<object> roleIds, object? organizationId = null)
    {
        id = id.Transform();
        organizationId = organizationId.Transform();
        var roleIdsTransformed = roleIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.User.Schema,
            Table = DataSourcesConfig.UserRoleRelationsTable,
            Data = new DataSourceDictionary { { "user_id", id }, }
        };

        if (Options.Organization != null && organizationId != null)
            input.Data["organization_id"] = organizationId;

        foreach (var roleId in roleIdsTransformed)
        {
            input.Data["role_id"] = roleId;
            Context.Create(input);
        }
    }

    public void UserRolesRemove(
        object? id,
        IEnumerable<object> roleIds,
        object? organizationId = null
    )
    {
        id = id.Transform();
        organizationId = organizationId.Transform();
        var roleIdsTransformed = roleIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.User.Schema,
            Table = DataSourcesConfig.UserRoleRelationsTable,
            Alias = "user_role",
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = "user_role",
                        Column = "user_id",
                        Operator = EOperator.Equals,
                        Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = "user_role",
                        Column = "role_id",
                        Operator = EOperator.In,
                        Value = roleIdsTransformed
                    }
                }
            }
        };

        if (Options.Organization != null && organizationId != null)
            ((ConditionAndInput)input.Where).Conditions = (
                (ConditionAndInput)input.Where
            ).Conditions.Append(
                new ConditionValueInput
                {
                    Alias = "user_role",
                    Column = "organization_id",
                    Operator = EOperator.Equals,
                    Value = organizationId
                }
            );

        Context.Delete(input);
    }

    public void UserPermissionsAdd(
        object? id,
        IEnumerable<object> permissionIds,
        object? organizationId = null
    )
    {
        id = id.Transform();
        organizationId = organizationId.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.User.Schema,
            Table = DataSourcesConfig.UserPermissionRelationsTable,
            Data = new DataSourceDictionary { { "user_id", id }, }
        };

        if (Options.Organization != null && organizationId != null)
            input.Data["organization_id"] = organizationId;

        foreach (var permissionId in permissionIdsTransformed)
        {
            input.Data["permission_id"] = permissionId;
            Context.Create(input);
        }
    }

    public void UserPermissionsRemove(
        object? id,
        IEnumerable<object> permissionIds,
        object? organizationId = null
    )
    {
        id = id.Transform();
        organizationId = organizationId.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.User.Schema,
            Table = DataSourcesConfig.UserPermissionRelationsTable,
            Alias = DataSourcesConfig.UserPermissionRelationsTable,
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.UserPermissionRelationsTable,
                        Column = "user_id",
                        Operator = EOperator.Equals,
                        Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.UserPermissionRelationsTable,
                        Column = "permission_id",
                        Operator = EOperator.In,
                        Value = permissionIdsTransformed
                    }
                }
            }
        };

        if (Options.Organization != null && organizationId != null)
            ((ConditionAndInput)input.Where).Conditions = (
                (ConditionAndInput)input.Where
            ).Conditions.Append(
                new ConditionValueInput
                {
                    Alias = DataSourcesConfig.UserPermissionRelationsTable,
                    Column = "organization_id",
                    Operator = EOperator.Equals,
                    Value = organizationId
                }
            );

        Context.Delete(input);
    }

    public UserDictionary RefreshTokenFindValid(string token)
    {
        var user = Context.SelectSingle<UserDictionary>(
            new SelectInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.RefreshToken.Table,
                Alias = Options.User.RefreshToken.Table,
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = Options.User.RefreshToken.Table,
                            Column = Options.User.RefreshToken.ColumnToken,
                            Operator = EOperator.Equals,
                            Value = token
                        },
                        new ConditionValueInput
                        {
                            Alias = Options.User.RefreshToken.Table,
                            Column = Options.User.RefreshToken.ColumnExpires,
                            Operator = EOperator.GreaterThanEquals,
                            Value = DateTime.UtcNow
                        }
                    }
                }
            }
        );

        return user;
    }

    public UserDictionary RefreshTokenCreate(object? userId, string token, DateTime expires)
    {
        userId = userId.Transform();

        var user = Context.Create<UserDictionary>(
            new CreateInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.RefreshToken.Table,
                Data = new DataSourceDictionary
                {
                    { Options.User.RefreshToken.ColumnUserId, userId },
                    { Options.User.RefreshToken.ColumnToken, token },
                    { Options.User.RefreshToken.ColumnExpires, expires },
                }
            }
        );

        return user;
    }

    public void RefreshTokenDelete(string token) =>
        Context.Delete(
            new DeleteInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.RefreshToken.Table,
                Alias = Options.User.RefreshToken.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.RefreshToken.Table,
                    Column = Options.User.RefreshToken.ColumnToken,
                    Operator = EOperator.Equals,
                    Value = token
                },
            }
        );

    public void RefreshTokensDelete(object? userId)
    {
        userId = userId.Transform();

        Context.Delete(
            new DeleteInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.RefreshToken.Table,
                Alias = Options.User.RefreshToken.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.RefreshToken.Table,
                    Column = Options.User.RefreshToken.ColumnUserId,
                    Operator = EOperator.Equals,
                    Value = userId
                },
            }
        );
    }

    public void RefreshTokenDeleteExpired() =>
        Context.Delete(
            new DeleteInput
            {
                Schema = Options.User.Schema,
                Table = Options.User.RefreshToken.Table,
                Alias = Options.User.RefreshToken.Table,
                Where = new ConditionValueInput
                {
                    Alias = Options.User.RefreshToken.Table,
                    Column = Options.User.RefreshToken.ColumnExpires,
                    Operator = EOperator.LessThan,
                    Value = DateTime.UtcNow
                }
            }
        );

    public override void SupportTableSetup()
    {
        var userColumnId = GetColumnId(
            Options.User.Schema,
            Options.User.Table,
            Options.User.ColumnId
        );

        Context.CreateTable(
            new TableInput
            {
                Schema = Options.User.Schema,
                Name = Options.User.Support.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnBlocked,
                        Type = EColumnType.Boolean,
                        NotNull = true
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnAdminAccess,
                        Type = EColumnType.Boolean,
                        NotNull = true
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnPasswordResetToken,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnSalt,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnFbAccessCode,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnGoogleAccessCode,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnAppleAccessCode,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.Support.ColumnRelatedEntityId,
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
                        Name = $"{Options.User.Support.KeyPrefix}__user_id",
                        Columns = new[] { Options.User.Support.ColumnRelatedEntityId, }
                    }
                }
            },
            true
        );

        Context.CreateTable(
            new TableInput
            {
                Schema = Options.User.Schema,
                Name = EntityMetaOptions!.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = EntityMetaOptions.ColumnKey,
                        Type = EColumnType.Varchar,
                        NotNull = true
                    },
                    new ColumnInput
                    {
                        Name = EntityMetaOptions.ColumnValue,
                        Type = EColumnType.Text,
                        NotNull = true
                    },
                    new ColumnInput
                    {
                        Name = EntityMetaOptions.ColumnRelatedEntityId,
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
                }
            },
            true
        );

        Context.CreateTable(
            new TableInput
            {
                Schema = Options.User.Schema,
                Name = Options.User.RefreshToken.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = Options.User.RefreshToken.ColumnToken,
                        Type = EColumnType.Text
                    },
                    new ColumnInput
                    {
                        Name = Options.User.RefreshToken.ColumnExpires,
                        Type = EColumnType.DateTime
                    },
                    new ColumnInput
                    {
                        Name = Options.User.RefreshToken.ColumnUserId,
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
                }
            },
            true
        );

        Context.CreateTable(
            new TableInput
            {
                Schema = Options.User.Schema,
                Name = Options.User.AppleData.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = Options.User.AppleData.ColumnId,
                        Type = EColumnType.BigSerial
                    },
                    new ColumnInput
                    {
                        Name = Options.User.AppleData.ColumnCode,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.AppleData.ColumnEmail,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.AppleData.ColumnFirstName,
                        Type = EColumnType.Varchar
                    },
                    new ColumnInput
                    {
                        Name = Options.User.AppleData.ColumnLastName,
                        Type = EColumnType.Varchar
                    },
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{Options.User.AppleData.KeyPrefix}__id",
                        Columns = new[] { Options.User.AppleData.ColumnId }
                    }
                }
            },
            true
        );

        if (Options.Role != null)
        {
            var roleColumnId = GetColumnId(
                Options.Role.Schema,
                Options.Role.Table,
                Options.Role.ColumnId
            );

            var input = new TableInput
            {
                Schema = Options.User.Schema,
                Name = DataSourcesConfig.UserRoleRelationsTable,
                Columns = new[]
                {
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
                    },
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
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{DataSourcesConfig.UserRoleRelationsPrimaryKey}__user_role_id",
                        Columns = new[] { "user_id", "role_id", }
                    }
                }
            };

            if (Options.Organization != null)
            {
                var organizationColumnId = GetColumnId(
                    Options.Organization.Schema,
                    Options.Organization.Table,
                    Options.Organization.ColumnId
                );

                input.Columns = input.Columns.Append(
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
                    }
                );

                input.PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name =
                            $"{DataSourcesConfig.UserRoleRelationsPrimaryKey}__organization_user_role_id",
                        Columns = new[] { "organization_id", "user_id", "role_id", }
                    }
                };
            }

            Context.CreateTable(input, true);
        }

        if (Options.Permission != null)
        {
            var permissionColumnId = GetColumnId(
                Options.Permission.Schema,
                Options.Permission.Table,
                Options.Permission.ColumnId
            );

            var input = new TableInput
            {
                Schema = Options.User.Schema,
                Name = DataSourcesConfig.UserPermissionRelationsTable,
                Columns = new[]
                {
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
                            OnDelete = ActionType.Cascade
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
                        Name =
                            $"{DataSourcesConfig.UserPermissionRelationsPrimaryKey}__user_permission_id",
                        Columns = new[] { "user_id", "permission_id" }
                    }
                }
            };

            if (Options.Organization != null)
            {
                var organizationColumnId = GetColumnId(
                    Options.Organization.Schema,
                    Options.Organization.Table,
                    Options.Organization.ColumnId
                );

                input.Columns = input.Columns.Append(
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
                    }
                );

                input.PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name =
                            $"{DataSourcesConfig.UserPermissionRelationsPrimaryKey}__organization_user_permission_id",
                        Columns = new[] { "organization_id", "user_id", "permission_id" }
                    }
                };
            }

            Context.CreateTable(input, true);
        }
    }
}
