using Clouded.Auth.Provider.Config;
using Clouded.Auth.Provider.DataSources.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Shared.Enums;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.DataSources;

public class DomainDataSource(ApplicationOptions options, IHashService hashService)
    : BaseDataSource<
        DomainDictionary,
        EntityIdentityDomainOptions,
        IdentityDomainMetaOptions,
        EntitySupportOptions
    >(
        options,
        hashService,
        new[]
        {
            options.Clouded.Auth.Identity.Domain.ColumnId,
            options.Clouded.Auth.Identity.Domain.ColumnIdentity
        },
        options.Clouded.Auth.Identity.Domain,
        null,
        options.Clouded.Auth.Identity.Domain?.Support
    ),
        IDomainDataSource
{
    public override DomainDictionary EntityCreate(
        DomainDictionary data,
        DataSourceDictionary? supportData = null
    )
    {
        if (Options.Domain == null)
            throw new NotSupportedException();

        var domain = Context.Create<DomainDictionary>(
            new CreateInput
            {
                Schema = Options.Domain.Schema,
                Table = Options.Domain.Table,
                ReturnColumns = new[] { "*" },
                Data = data
            }
        );

        return domain;
    }

    public void DomainExtendWithMachines(DomainDictionary domain)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        domain.Add("clouded_machines", DomainMachines(domain.Id));
    }

    public IEnumerable<MachineDictionary> DomainMachines(object domainId)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        domainId = domainId.Transform();

        var input = new SelectInput
        {
            Schema = Options.Machine.Schema,
            Table = Options.Machine.Table,
            Alias = Options.Machine.Table,
            Join = new JoinInput[]
            {
                new()
                {
                    Schema = Options.Domain.Schema,
                    Table = DataSourcesConfig.MachineDomainRelationsTable,
                    Alias = DataSourcesConfig.MachineDomainRelationsTable,
                    On = new ConditionColumnInput
                    {
                        Alias = Options.Machine.Table,
                        Column = Options.Machine.ColumnId,
                        TargetAlias = DataSourcesConfig.MachineDomainRelationsTable,
                        TargetColumn = "machine_id",
                        Operator = EOperator.Equals
                    }
                }
            },
            Where = new ConditionValueInput
            {
                Alias = DataSourcesConfig.MachineDomainRelationsTable,
                Column = "domain_id",
                Value = domainId,
                Operator = EOperator.Equals
            }
        };

        var machines = Context.Select<MachineDictionary>(input);
        return machines;
    }

    public void DomainMachinesAdd(object id, IEnumerable<object> machineIds)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        id = id.Transform();
        var machineIdsTransformed = machineIds.Transform();

        var input = new CreateInput
        {
            Schema = Options.Domain.Schema,
            Table = DataSourcesConfig.MachineDomainRelationsTable,
            Data = new BaseDictionary { { "domain_id", id }, }
        };

        foreach (var machineId in machineIdsTransformed)
        {
            input.Data["machine_id"] = machineId;
            Context.Create<BaseDictionary>(input);
        }
    }

    public void DomainMachinesRemove(object id, IEnumerable<object> machineIds)
    {
        if (Options.Machine == null)
            throw new NotSupportedException();

        id = id.Transform();
        var machineIdsTransformed = machineIds.Transform();

        var input = new DeleteInput
        {
            Schema = Options.Domain.Schema,
            Table = DataSourcesConfig.MachineDomainRelationsTable,
            Alias = DataSourcesConfig.MachineDomainRelationsTable,
            Where = new ConditionAndInput
            {
                Conditions = new[]
                {
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.MachineDomainRelationsTable,
                        Column = "domain_id",
                        Operator = EOperator.Equals,
                        Value = id
                    },
                    new ConditionValueInput
                    {
                        Alias = DataSourcesConfig.MachineDomainRelationsTable,
                        Column = "machine_id",
                        Operator = EOperator.In,
                        Value = machineIdsTransformed
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
        Context.CreateTable(
            new TableInput
            {
                Schema = Options.Domain.Schema,
                Name = Options.Domain.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = Options.Domain.ColumnId,
                        Type = EColumnType.BigSerial
                    },
                    new ColumnInput
                    {
                        Name = Options.Domain.ColumnIdentity,
                        Type = EColumnType.Varchar,
                        NotNull = true
                    },
                },
                UniqueKeys = new[]
                {
                    new ConstraintUniqueKeyInput
                    {
                        Name = $"{Options.Domain.KeyPrefix}__value",
                        Columns = new[] { Options.Domain.ColumnIdentity }
                    }
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{Options.Domain.KeyPrefix}__id",
                        Columns = new[] { Options.Domain.ColumnId }
                    }
                }
            },
            true
        );
    }

    public override DomainDictionary EntitySupportTable(object? entityId)
    {
        // Domain is not using support table
        throw new NotImplementedException();
    }

    public override DomainDictionary EntitySupportData(object? entityId)
    {
        // Domain is not using support table
        throw new NotImplementedException();
    }
}
