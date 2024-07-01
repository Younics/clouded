using Clouded.Auth.Provider.Config;
using Clouded.Auth.Provider.DataSources.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Pagination;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;
using Clouded.Shared;
using Clouded.Shared.Enums;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.DataSources;

public class MachineDataSource(ApplicationOptions options, IHashService hashService) :
    BaseDataSource<MachineDictionary, EntityIdentityMachineOptions, IdentityMachineMetaOptions, EntitySupportOptions>(
        options,
        hashService, options.Clouded.Auth.Identity.Machine == null
            ? Array.Empty<string>()
            : [
                options.Clouded.Auth.Identity.Machine.ColumnId, options.Clouded.Auth.Identity.Machine.ColumnIdentity
            ],
        options.Clouded.Auth.Identity.Machine, options.Clouded.Auth.Identity.Machine?.Meta,
        options.Clouded.Auth.Identity.Machine?.Support), IMachineDataSource
{
    public IEnumerable<MachineDictionary> Machines
    (
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    )
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        search = search.Transform();
        var roleIdsTransformed = roleIds?.Transform();
        var permissionIdsTransformed = permissionIds?.Transform();

        var join = new List<JoinInput>();
        var where = new List<ICondition>();

        if (Options.Role != null && roleIdsTransformed is { Count: > 0 })
        {
            join.Add
            (
                new JoinInput
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachineRoleRelationsTable,
                    Alias = "machine_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_role_relation",
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add
            (
                new ConditionValueInput
                {
                    Alias = "machine_role_relation",
                    Column = "role_id",
                    Operator = EOperator.In,
                    Value = roleIdsTransformed
                }
            );
        }

        if (Options.Permission != null && permissionIdsTransformed is { Count: > 0 })
        {
            join.Add
            (
                new JoinInput
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachinePermissionRelationsTable,
                    Alias = "machine_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_permission_relation",
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add
            (
                new ConditionValueInput
                {
                    Alias = "machine_permission_relation",
                    Column = "permission_id",
                    Operator = EOperator.In,
                    Value = permissionIdsTransformed
                }
            );
        }


        if (search != null)
        {
            where.Add
            (
                new ConditionValueInput
                {
                    Alias = Options.Machine.Table,
                    Column = Options.Machine.ColumnIdentity,
                    Operator = EOperator.Contains,
                    Mode = EMode.Insensitive,
                    Value = search
                }
            );
        }

        return GetAll
        (
            Options.Machine.Schema,
            Options.Machine.Table,
            Options.Machine.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { Options.Machine.Table, "*" } } },
            join.Any()
                ? join
                : null,
            where.Any()
                ? new ConditionAndInput { Conditions = where }
                : null,
            new[]
            {
                new OrderInput
                {
                    Alias = Options.Machine.Table,
                    Column = orderByColumn ?? Options.Machine.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            true
        );
    }

    public Paginated<MachineDictionary> MachinePaginated
    (
        int page,
        int size,
        object? search = null,
        IEnumerable<object>? roleIds = null,
        IEnumerable<object>? permissionIds = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    )
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        search = search.Transform();
        var roleIdsTransformed = roleIds?.Transform();
        var permissionIdsTransformed = permissionIds?.Transform();

        var join = new List<JoinInput>();
        var where = new List<ICondition>();

        if (Options.Role != null && roleIdsTransformed is { Count: > 0 })
        {
            join.Add
            (
                new JoinInput
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachineRoleRelationsTable,
                    Alias = "machine_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_role_relation",
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add
            (
                new ConditionValueInput
                {
                    Alias = "machine_role_relation",
                    Column = "role_id",
                    Operator = EOperator.In,
                    Value = roleIdsTransformed
                }
            );
        }

        if (Options.Permission != null && permissionIdsTransformed is { Count: > 0 })
        {
            join.Add
            (
                new JoinInput
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachinePermissionRelationsTable,
                    Alias = "machine_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_permission_relation",
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                }
            );

            where.Add
            (
                new ConditionValueInput
                {
                    Alias = "machine_permission_relation",
                    Column = "permission_id",
                    Operator = EOperator.In,
                    Value = permissionIdsTransformed
                }
            );
        }

        if (search != null)
        {
            where.Add
            (
                new ConditionValueInput
                {
                    Alias = Options.Machine.Table,
                    Column = Options.Machine.ColumnIdentity,
                    Operator = EOperator.Contains,
                    Mode = EMode.Insensitive,
                    Value = search
                }
            );
        }

        return GetPaginated
        (
            Options.Machine.Schema,
            Options.Machine.Table,
            Options.Machine.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { Options.Machine.Table, "*" } } },
            page,
            size,
            join,
            where.Any()
                ? new ConditionAndInput { Conditions = where }
                : null,
            new[]
            {
                new OrderInput
                {
                    Alias = Options.Machine.Table,
                    Column = orderByColumn ?? Options.Machine.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            distinct: true
        );
    }

    public MachineDictionary MachineFindByKeys(string apiKey, string secretKey)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        var machine = Context.SelectSingle<MachineDictionary>
        (
            new SelectInput
            {
                Schema = Options.Machine.Schema,
                Table = Options.Machine.Table,
                Alias = Options.Machine.Table,
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = Options.Machine.Table,
                            Column = Options.Machine.ColumnApiKey,
                            Operator = EOperator.Equals,
                            Value = apiKey
                        },
                        new ConditionValueInput
                        {
                            Alias = Options.Machine.Table,
                            Column = Options.Machine.ColumnSecretKey,
                            Operator = EOperator.Equals,
                            Value = secretKey
                        }
                    }
                }
            }
        );

        return machine;
    }

    public override MachineDictionary EntityCreate(MachineDictionary data, DataSourceDictionary? supportData = null)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        data[Options.Machine.ColumnBlocked] = false;
        data[Options.Machine.ColumnApiKey] =
            $"{Generator.RandomString(32, includeSpecialCharacters: false)}" +
            $"{DateTime.UtcNow.Ticks}" +
            $"{Generator.RandomString(32, includeSpecialCharacters: false)}";
        data[Options.Machine.ColumnSecretKey] = Generator.RandomString(48, includeSpecialCharacters: false);

        var machine = Context.Create<MachineDictionary>
        (
            new CreateInput { Schema = Options.Machine.Schema, Table = Options.Machine.Table, ReturnColumns = new[] { "*" }, Data = data }
        );

        return machine;
    }

    public IEnumerable<RoleDictionary> MachineRoles(object? machineId)
    {
        if (Options.Machine == null)
            return Array.Empty<RoleDictionary>();
        if (Options.Role == null)
            throw new NotSupportedException();

        machineId = machineId.Transform();

        var input = new SelectInput
        {
            Schema = Options.Role.Schema,
            Table = Options.Role.Table,
            Alias = "role",
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachineRoleRelationsTable,
                    Alias = "machine_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = "role",
                        Column = Options.Role.ColumnId,
                        TargetAlias = "machine_role_relation",
                        TargetColumn = "role_id",
                        Operator = EOperator.Equals
                    }
                }
            },
            Where = new ConditionValueInput
            {
                Alias = "machine_role_relation",
                Column = "machine_id",
                Value = machineId,
                Operator = EOperator.Equals
            }
        };

        var roles = Context.Select<RoleDictionary>(input);
        return roles;
    }

    public IEnumerable<PermissionDictionary> MachinePermissions(object? machineId)
    {
        if (Options.Machine == null)
            return Array.Empty<PermissionDictionary>();
        if (Options.Permission == null)
            throw new NotSupportedException();

        machineId = machineId.Transform();

        var input = new SelectInput
        {
            Schema = Options.Permission.Schema,
            Table = Options.Permission.Table,
            Alias = Options.Permission.Table,
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachinePermissionRelationsTable,
                    Alias = "machine_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Permission.Table,
                        Column = Options.Permission.ColumnId,
                        TargetAlias = "machine_permission_relation",
                        TargetColumn = "permission_id",
                        Operator = EOperator.Equals
                    }
                }
            },
            Where = new ConditionValueInput
            {
                Alias = "machine_permission_relation",
                Column = "machine_id",
                Value = machineId,
                Operator = EOperator.Equals
            }
        };

        var permissions = Context.Select<PermissionDictionary>(input);
        return permissions;
    }

    public IEnumerable<DomainDictionary> MachineDomains(object? machineId)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        machineId = machineId.Transform();

        var input = new SelectInput
        {
            Schema = Options.Domain.Schema,
            Table = Options.Domain.Table,
            Alias = Options.Domain.Table,
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachineDomainRelationsTable,
                    Alias = DataSourcesConfig.MachineDomainRelationsTable,
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Domain.Table,
                        Column = Options.Domain.ColumnId,
                        TargetAlias = DataSourcesConfig.MachineDomainRelationsTable,
                        TargetColumn = "domain_id",
                        Operator = EOperator.Equals
                    }
                }
            },
            Where = new ConditionValueInput
            {
                Alias = DataSourcesConfig.MachineDomainRelationsTable,
                Column = "machine_id",
                Value = machineId,
                Operator = EOperator.Equals
            }
        };

        var domains = Context.Select<DomainDictionary>(input);
        return domains;
    }

    public IEnumerable<PermissionDictionary> MachinePermissionsWithRolePermissions(object? machineId)
    {
        machineId = machineId.Transform();

        if (Options.Role == null)
            return MachinePermissions(machineId);

        if (Options.Machine == null)
            return Array.Empty<PermissionDictionary>();
        if (Options.Permission == null)
            throw new NotSupportedException();

        var input = new SelectInput
        {
            Schema = Options.Machine.Schema,
            Table = Options.Machine.Table,
            Alias = Options.Machine.Table,
            Distinct = true,
            SelectedColumns =
                new SelectColDesc[]
                {
                    new() { ColJoin = new[] { Options.Permission.Table, Options.Permission.ColumnId } },
                    new() { ColJoin = new[] { Options.Permission.Table, Options.Permission.ColumnIdentity } }
                },
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachineRoleRelationsTable,
                    Alias = "machine_role_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_role_relation",
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                },
                new()
                {
                    Schema = Options.Machine.Schema,
                    Table = DataSourcesConfig.MachinePermissionRelationsTable,
                    Alias = "machine_permission_relation",
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = "machine_permission_relation",
                        TargetColumn = "machine_id",
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
                        Alias = "machine_role_relation",
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
                                TargetAlias = "machine_permission_relation",
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
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        Value = machineId,
                        Operator = EOperator.Equals
                    }
                }
            }
        };

        var permissions = Context
            .Select<PermissionDictionary>(input)
            .Where
            (
                permission =>
                    permission[Options.Permission.ColumnIdentity] != null
            );

        return permissions;
    }

    public void MachineExtendWithRoles(MachineDictionary machine)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        if (Options.Role == null || !machine.ContainsKey(Options.Machine.ColumnId))
            return;

        machine.Add("clouded_roles", MachineRoles(machine.Id));
    }

    public void MachineExtendWithPermissions(MachineDictionary machine)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        if (Options.Permission == null || !machine.ContainsKey(Options.Machine.ColumnId))
            return;

        machine.Add("clouded_permissions", MachinePermissionsWithRolePermissions(machine.Id));
    }

    public void MachineExtendWithDomains(MachineDictionary machine)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        machine.Add("clouded_domains", MachineDomains(machine.Id));
    }


    public void MachineRolesAdd(object? id, IEnumerable<object> roleIds)
    {
        if (Options.Machine == null || Options.Role == null)
            throw new NotSupportedException();

        id = id.Transform();
        var roleIdsTransformed = roleIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachineRoleRelationsTable,
            Data = new MachineDictionary { { "machine_id", id }, }
        };

        foreach (var roleId in roleIdsTransformed)
        {
            input.Data["role_id"] = roleId;
            Context.Create<RoleDictionary>(input);
        }
    }

    public void MachineRolesRemove(object? id, IEnumerable<object> roleIds)
    {
        if (Options.Machine == null || Options.Role == null)
            throw new NotSupportedException();

        id = id.Transform();
        var roleIdsTransformed = roleIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachineRoleRelationsTable,
            Alias = "machine_role",
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = "machine_role", Column = "machine_id", Operator = EOperator.Equals, Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = "machine_role", Column = "role_id", Operator = EOperator.In, Value = roleIdsTransformed
                    }
                }
            }
        };

        Context.Delete(input);
    }

    public void MachinePermissionsAdd(object? id, IEnumerable<object> permissionIds)
    {
        if (Options.Machine == null || Options.Role == null)
            throw new NotSupportedException();

        id = id.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachinePermissionRelationsTable,
            Data = new MachineDictionary { { "machine_id", id }, }
        };

        foreach (var permissionId in permissionIdsTransformed)
        {
            input.Data["permission_id"] = permissionId;
            Context.Create<PermissionDictionary>(input);
        }
    }

    public void MachinePermissionsRemove(object? id, IEnumerable<object> permissionIds)
    {
        if (Options.Machine == null || Options.Role == null)
            throw new NotSupportedException();

        id = id.Transform();
        var permissionIdsTransformed = permissionIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachinePermissionRelationsTable,
            Alias = "machine_permission",
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = "machine_permission", Column = "machine_id", Operator = EOperator.Equals, Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = "machine_permission",
                        Column = "permission_id",
                        Operator = EOperator.In,
                        Value = permissionIdsTransformed
                    }
                }
            }
        };

        Context.Delete(input);
    }

    public void MachineDomainsAdd(object? id, IEnumerable<object> domainIds)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        id = id.Transform();
        var domainIdsTransformed = domainIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachineDomainRelationsTable,
            Data = new BaseDictionary { { "machine_id", id }, }
        };

        foreach (var domainId in domainIdsTransformed)
        {
            input.Data["domain_id"] = domainId;
            Context.Create<BaseDictionary>(input);
        }
    }

    public void MachineDomainsRemove(object? id, IEnumerable<object> domainIds)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        id = id.Transform();
        var domainIdsTransformed = domainIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.Machine.Schema,
            Table = DataSourcesConfig.MachineDomainRelationsTable,
            Alias = DataSourcesConfig.MachineDomainRelationsTable,
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.MachineDomainRelationsTable, Column = "machine_id", Operator = EOperator.Equals, Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.MachineDomainRelationsTable,
                        Column = "domain_id",
                        Operator = EOperator.In,
                        Value = domainIdsTransformed
                    }
                }
            }
        };

        Context.Delete(input);
    }

    public override void TableVerification()
    {
        // Skip validation, because create is using IfNotExists flag  
    }

    public override void SupportTableSetup()
    {
        if (Options.Machine == null)
            return;

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Machine.Schema,
                Name = Options.Machine.Table,
                Columns = new[]
                {
                    new ColumnInput { Name = Options.Machine.ColumnId, Type = EColumnType.BigSerial },
                    new ColumnInput { Name = Options.Machine.ColumnIdentity, Type = EColumnType.Varchar, NotNull = true },
                    new ColumnInput { Name = Options.Machine.ColumnDescription, Type = EColumnType.Text },
                    new ColumnInput { Name = Options.Machine.ColumnApiKey, Type = EColumnType.Varchar },
                    new ColumnInput { Name = Options.Machine.ColumnSecretKey, Type = EColumnType.Varchar },
                    new ColumnInput { Name = Options.Machine.ColumnExpiresIn, Type = EColumnType.Double },
                    new ColumnInput { Name = Options.Machine.ColumnBlocked, Type = EColumnType.Boolean, NotNull = true },
                    new ColumnInput { Name = Options.Machine.ColumnType, Type = EColumnType.Varchar, NotNull = true },
                },
                UniqueKeys = new[]
                {
                    new ConstraintUniqueKeyInput
                    {
                        Name = $"{Options.Machine.KeyPrefix}__token",
                        Columns = new[] { Options.Machine.ColumnApiKey, Options.Machine.ColumnSecretKey }
                    }
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{Options.Machine.KeyPrefix}__id", Columns = new[] { Options.Machine.ColumnId }
                    }
                }
            },
            true
        );

        var machineColumnId = GetColumnId
        (
            Options.Machine.Schema,
            Options.Machine.Table,
            Options.Machine.ColumnId
        );

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Machine.Schema,
                Name = EntityMetaOptions!.Table,
                Columns = new[]
                {
                    new ColumnInput { Name = EntityMetaOptions.ColumnKey, Type = EColumnType.Varchar, NotNull = true },
                    new ColumnInput { Name = EntityMetaOptions.ColumnValue, Type = EColumnType.Text, NotNull = true }, new ColumnInput
                    {
                        Name = EntityMetaOptions.ColumnRelatedEntityId,
                        TypeRaw = machineColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "machine",
                            TargetSchema = Options.Machine.Schema,
                            TargetTable = Options.Machine.Table,
                            TargetColumn = Options.Machine.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    }
                }
            },
            true
        );

        var domainColumnId = GetColumnId
        (
            Options.Domain.Schema,
            Options.Domain.Table,
            Options.Domain.ColumnId
        );

        Context.CreateTable
        (
            new TableInput
            {
                Schema = Options.Machine.Schema,
                Name = DataSourcesConfig.MachineDomainRelationsTable,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = "machine_id",
                        TypeRaw = machineColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "machine",
                            TargetSchema = Options.Machine.Schema,
                            TargetTable = Options.Machine.Table,
                            TargetColumn = machineColumnId.Name,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    },
                    new ColumnInput
                    {
                        Name = "domain_id",
                        TypeRaw = domainColumnId.TypeRaw,
                        Reference = new ConstraintReferenceInput
                        {
                            Name = "domain",
                            TargetSchema = Options.Domain.Schema,
                            TargetTable = Options.Domain.Table,
                            TargetColumn = Options.Domain.ColumnId,
                            OnUpdate = ActionType.NoAction,
                            OnDelete = ActionType.Cascade,
                        }
                    },
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{DataSourcesConfig.MachineDomainRelationsPrimaryKey}__machine_domain_id",
                        Columns = new[] { "machine_id", "domain_id", }
                    }
                }
            },
            true
        );

        if (Options.Role != null)
        {
            var roleColumnId = GetColumnId
            (
                Options.Role.Schema,
                Options.Role.Table,
                Options.Role.ColumnId
            );

            Context.CreateTable
            (
                new TableInput
                {
                    Schema = Options.Machine.Schema,
                    Name = DataSourcesConfig.MachineRoleRelationsTable,
                    Columns = new[]
                    {
                        new ColumnInput
                        {
                            Name = "machine_id",
                            TypeRaw = machineColumnId.TypeRaw,
                            Reference = new ConstraintReferenceInput
                            {
                                Name = "machine",
                                TargetSchema = Options.Machine.Schema,
                                TargetTable = Options.Machine.Table,
                                TargetColumn = machineColumnId.Name,
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
                            Name = $"{DataSourcesConfig.MachineRoleRelationsPrimaryKey}__machine_role_id",
                            Columns = new[] { "machine_id", "role_id", }
                        }
                    }
                },
                true
            );
        }

        if (Options.Permission != null)
        {
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
                    Schema = Options.Machine.Schema,
                    Name = DataSourcesConfig.MachinePermissionRelationsTable,
                    Columns = new[]
                    {
                        new ColumnInput
                        {
                            Name = "machine_id",
                            TypeRaw = machineColumnId.TypeRaw,
                            Reference = new ConstraintReferenceInput
                            {
                                Name = "machine",
                                TargetSchema = Options.Machine.Schema,
                                TargetTable = Options.Machine.Table,
                                TargetColumn = machineColumnId.Name,
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
                            Name = $"{DataSourcesConfig.MachinePermissionRelationsPrimaryKey}__machine_permission_id",
                            Columns = new[] { "machine_id", "permission_id" }
                        }
                    }
                },
                true
            );
        }
    }

    public override MachineDictionary EntitySupportTable(object? entityId)
    {
        // Machine is not using support table
        throw new NotImplementedException();
    }

    public override MachineDictionary EntitySupportData(object? entityId)
    {
        // Machine is not using support table
        throw new NotImplementedException();
    }
}
