using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Pagination;
using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;
using Clouded.Results.Exceptions;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;
using NotSupportedException = Clouded.Results.Exceptions.NotSupportedException;

namespace Clouded.Auth.Provider.DataSources.Base;

public abstract class BaseDataSource<T, TIdentityOptions, TMetaOptions, TSupportOptions>
    : IDataSource<T>
    where T : BaseDictionary, new()
    where TIdentityOptions : BaseEntityIdentityOptions
    where TMetaOptions : EntityMetaOptions
    where TSupportOptions : EntitySupportOptions
{
    protected readonly IdentityOptions Options;
    protected readonly Context Context;
    protected readonly IHashService HashService;
    protected readonly IEnumerable<string> EntityColumns;
    public readonly TIdentityOptions? EntityIdentityOptions;
    protected readonly TMetaOptions? EntityMetaOptions;
    protected readonly TSupportOptions? EntitySupportOptions;

    public BaseDataSource(
        ApplicationOptions options,
        IHashService hashService,
        IEnumerable<string> entityColumns,
        TIdentityOptions? entityIdentityOptions,
        TMetaOptions? entityMetaOptions,
        TSupportOptions? entitySupportOptions
    )
    {
        var dataSourceOptions = options.Clouded.DataSource;

        Options = options.Clouded.Auth.Identity;
        HashService = hashService;
        EntityColumns = entityColumns;
        EntityIdentityOptions = entityIdentityOptions;
        EntityMetaOptions = entityMetaOptions;
        EntitySupportOptions = entitySupportOptions;

        Context = new Context(
            new Connection
            {
                Type = dataSourceOptions.Type,
                Server = dataSourceOptions.Server,
                Port = dataSourceOptions.Port,
                Username = dataSourceOptions.Username,
                Password = dataSourceOptions.Password,
                Database = dataSourceOptions.Database
            }
        );
    }

    public BaseEntityIdentityOptions? GetBaseEntityIdentityOptions() => EntityIdentityOptions;

    public virtual IEnumerable<ColumnResult> GetEntityColumns()
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        return Context.GetColumns(EntityIdentityOptions.Schema, EntityIdentityOptions.Table);
    }

    public virtual IEnumerable<T> Entities(
        object? search = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    )
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        var join = new List<JoinInput>();
        var where = new List<ICondition>();

        search = search.Transform();

        if (search != null)
        {
            where.Add(
                new ConditionOrInput
                {
                    Conditions = EntityIdentityOptions.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = EntityIdentityOptions.Table,
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
            EntityIdentityOptions.Schema,
            EntityIdentityOptions.Table,
            EntityIdentityOptions.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { EntityIdentityOptions.Table, "*" } } },
            join.Any() ? join : null,
            where.Any() ? new ConditionAndInput { Conditions = where } : null,
            new[]
            {
                new OrderInput
                {
                    Alias = EntityIdentityOptions.Table,
                    Column = orderByColumn ?? EntityIdentityOptions.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            true
        );
    }

    public virtual Paginated<T> EntitiesPaginated(
        int page,
        int size,
        object? search = null,
        (IEnumerable<ColumnResult>, Dictionary<string, object?>)? filter = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    )
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        search = search.Transform();

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
                                            EntityIdentityOptions.Table,
                                            filter.Value.Item2[col.GetKey()]!
                                        )
                                )
                                .ToList()
                        }
                }
            }
        };

        if (search != null)
        {
            where.Add(
                new ConditionOrInput
                {
                    Conditions = EntityIdentityOptions.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = EntityIdentityOptions.Table,
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
            EntityIdentityOptions.Schema,
            EntityIdentityOptions.Table,
            EntityIdentityOptions.Table,
            new SelectColDesc[] { new() { ColJoin = new[] { EntityIdentityOptions.Table, "*" } } },
            page,
            size,
            join,
            where.Any() ? new ConditionAndInput { Conditions = where } : null,
            new[]
            {
                new OrderInput
                {
                    Alias = EntityIdentityOptions.Table,
                    Column = orderByColumn ?? EntityIdentityOptions.ColumnId,
                    Direction = orderByDescending ? OrderType.Desc : OrderType.Asc
                }
            },
            distinct: true
        );
    }

    public virtual T EntityCreate(T data, DataSourceDictionary? supportData = null)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        var columns = GetEntityColumns();

        data = data.Transform(columns);

        var entity = Context.Create<T>(
            new CreateInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                ReturnColumns = new[] { "*" },
                Data = data
            }
        );

        return entity;
    }

    public virtual T EntityUpdate(object? id, T data, DataSourceDictionary? supportData = null)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        var columns = GetEntityColumns();

        id = id.Transform();
        data = data.Transform(columns);

        var entity = Context.Update<T>(
            new UpdateInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                ReturnColumns = new[] { "*" },
                Data = data,
                Where = new ConditionValueInput
                {
                    Alias = EntityIdentityOptions.Table,
                    Column = EntityIdentityOptions.ColumnId,
                    Operator = EOperator.Equals,
                    Value = id
                }
            }
        );

        return entity;
    }

    public virtual void EntityDelete(object? id)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        id = id.Transform();

        Context.Delete(
            new DeleteInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntityIdentityOptions.Table,
                    Column = EntityIdentityOptions.ColumnId,
                    Operator = EOperator.Equals,
                    Value = id
                }
            }
        );
    }

    public virtual bool EntityExists(T data, bool conditionsJoinedWithOr = false)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        var columns = GetEntityColumns();

        data = data.Transform(columns);

        return Context.Exists(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                Where = conditionsJoinedWithOr
                    ? new ConditionOrInput
                    {
                        Conditions = data.Select(
                            x =>
                                new ConditionValueInput
                                {
                                    Alias = EntityIdentityOptions.Table,
                                    Column = x.Key,
                                    Operator = EOperator.Equals,
                                    Value = x.Value
                                }
                        )
                    }
                    : new ConditionAndInput
                    {
                        Conditions = data.Select(
                            x =>
                                new ConditionValueInput
                                {
                                    Alias = EntityIdentityOptions.Table,
                                    Column = x.Key,
                                    Operator = EOperator.Equals,
                                    Value = x.Value
                                }
                        )
                    }
            }
        );
    }

    public virtual T EntityFindById(object? id)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        id = id.Transform();

        var entity = Context.SelectSingle<T>(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntityIdentityOptions.Table,
                    Column = EntityIdentityOptions.ColumnId,
                    Operator = EOperator.Equals,
                    Value = id
                }
            }
        );

        return entity;
    }

    public virtual T EntityFindByIdentity(object? identity)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        identity = identity.Transform();

        var entity = Context.SelectSingle<T>(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                Where = new ConditionOrInput
                {
                    Conditions = EntityIdentityOptions.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = EntityIdentityOptions.Table,
                                Column = column,
                                Operator = EOperator.Equals,
                                Mode = EMode.Insensitive,
                                Value = identity
                            }
                    )
                }
            }
        );

        return entity;
    }

    public virtual IEnumerable<T> EntitySearch(object? value)
    {
        if (EntityIdentityOptions == null)
            throw new NotSupportedException();

        value = value.Transform();

        var entities = Context.Select<T>(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityIdentityOptions.Table,
                Alias = EntityIdentityOptions.Table,
                Where = new ConditionOrInput
                {
                    Conditions = EntityIdentityOptions.ColumnIdentityArray.Select(
                        column =>
                            new ConditionValueInput
                            {
                                Alias = EntityIdentityOptions.Table,
                                Column = column,
                                Operator = EOperator.Contains,
                                Mode = EMode.Insensitive,
                                Value = value
                            }
                    )
                }
            }
        );

        return entities;
    }

    public virtual void EntityMetaDataUpdate(object? entityId, Dictionary<string, object?> data)
    {
        if (EntityIdentityOptions == null || EntityMetaOptions == null)
            throw new NotSupportedException();

        entityId = entityId.Transform();
        data = data.Transform();

        Context.Delete(
            new DeleteInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntityMetaOptions.Table,
                Alias = EntityMetaOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntityMetaOptions.Table,
                    Column = EntityMetaOptions.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = entityId
                }
            }
        );

        foreach (var meta in data)
        {
            Context.Create<T>(
                new CreateInput
                {
                    Schema = EntityIdentityOptions.Schema,
                    Table = EntityMetaOptions.Table,
                    Data = new DataSourceDictionary
                    {
                        { EntityMetaOptions.ColumnKey, meta.Key },
                        { EntityMetaOptions.ColumnValue, meta.Value },
                        { EntityMetaOptions.ColumnRelatedEntityId, entityId }
                    }
                }
            );
        }
    }

    public virtual Dictionary<string, object?> EntityMetaData(object? entityId)
    {
        if (EntityIdentityOptions == null || EntityMetaOptions == null)
            throw new NotSupportedException();

        entityId = entityId.Transform();

        var metaData = Context
            .Select(
                new SelectInput
                {
                    Schema = EntityIdentityOptions.Schema,
                    Table = EntityMetaOptions.Table,
                    Alias = EntityMetaOptions.Table,
                    Where = new ConditionValueInput
                    {
                        Alias = EntityMetaOptions.Table,
                        Column = EntityMetaOptions.ColumnRelatedEntityId,
                        Value = entityId,
                        Operator = EOperator.Equals
                    }
                }
            )
            .ToDictionary(
                meta => meta.GetValueOrDefault(EntityMetaOptions.ColumnKey)!.ToString()!,
                meta => meta.GetValueOrDefault(EntityMetaOptions.ColumnValue)
            );

        return metaData;
    }

    public virtual T EntitySupportData(object? entityId)
    {
        if (EntityIdentityOptions == null || EntitySupportOptions == null)
            throw new NotSupportedException();

        entityId = entityId.Transform();

        var entity = Context.SelectSingle<T>(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntitySupportOptions.Table,
                Alias = EntitySupportOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntitySupportOptions.Table,
                    Column = EntitySupportOptions.ColumnRelatedEntityId,
                    Value = entityId,
                    Operator = EOperator.Equals
                }
            }
        );

        return entity;
    }

    public virtual T EntitySupportTable(object? entityId)
    {
        if (EntityIdentityOptions == null || EntitySupportOptions == null)
            throw new NotSupportedException();

        entityId = entityId.Transform();

        var entity = Context.SelectSingle<T>(
            new SelectInput
            {
                Schema = EntityIdentityOptions.Schema,
                Table = EntitySupportOptions.Table,
                Alias = EntitySupportOptions.Table,
                Where = new ConditionValueInput
                {
                    Alias = EntitySupportOptions.Table,
                    Column = EntitySupportOptions.ColumnRelatedEntityId,
                    Operator = EOperator.Equals,
                    Value = entityId
                }
            }
        );

        return entity;
    }

    public virtual void TableVerification()
    {
        if (EntityIdentityOptions != null)
        {
            TableVerification(
                EntityIdentityOptions.Schema,
                EntityIdentityOptions.Table,
                EntityColumns
            );
        }
    }

    public abstract void SupportTableSetup();

    private void TableVerification(string schema, string table, IEnumerable<string> columns)
    {
        var existingColumns = Context.GetColumns(schema, table).Select(x => x.Name).ToList();

        foreach (var column in columns)
        {
            if (column.Contains(','))
            {
                var virtualColumns = column.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var virtualColumn in virtualColumns)
                {
                    if (!existingColumns.Contains(virtualColumn))
                        throw new ColumnNotFoundException(virtualColumn, $"{schema}.{table}");
                }
            }
            else
            {
                if (!existingColumns.Contains(column))
                    throw new ColumnNotFoundException(column, $"{schema}.{table}");
            }
        }
    }

    public ColumnResult GetColumnId(string schema, string table, string column) =>
        Context.GetColumns(schema, table).First(x => x.Name == column);

    protected Paginated<T> GetPaginated(
        string schema,
        string table,
        string alias,
        IEnumerable<SelectColDesc> selectedColumns,
        int page,
        int size,
        IEnumerable<JoinInput>? join = null,
        ICondition? where = null,
        IEnumerable<OrderInput>? order = null,
        bool distinct = false
    )
    {
        var selectedColumnsEnumerated = selectedColumns.ToList();

        var totalCount = Context.Count(
            new SelectInput
            {
                Distinct = distinct,
                Schema = schema,
                Table = table,
                Alias = alias,
                SelectedColumns = selectedColumnsEnumerated,
                Join = join ?? Array.Empty<JoinInput>(),
                Where = where
            }
        );

        var entities = Context.Select<T>(
            new SelectInput
            {
                Distinct = distinct,
                Schema = schema,
                Table = table,
                Alias = alias,
                SelectedColumns = selectedColumnsEnumerated,
                Join = join ?? Array.Empty<JoinInput>(),
                Where = where,
                OrderBy = order ?? Array.Empty<OrderInput>(),
                Offset = page * size,
                Limit = size
            }
        );

        return new Paginated<T>
        {
            Items = entities,
            PageInfo = new PageInfo
            {
                PageIndex = page,
                PageSize = size,
                TotalElements = totalCount,
                TotalPages = (long)Math.Ceiling((decimal)totalCount / size)
            }
        };
    }

    protected IEnumerable<T> GetAll(
        string schema,
        string table,
        string alias,
        IEnumerable<SelectColDesc> selectedColumns,
        IEnumerable<JoinInput>? join = null,
        ICondition? where = null,
        IEnumerable<OrderInput>? order = null,
        bool distinct = false
    )
    {
        var input = new SelectInput
        {
            Distinct = distinct,
            Schema = schema,
            Table = table,
            Alias = alias,
            SelectedColumns = selectedColumns,
        };

        if (where != null)
            input.Where = where;

        if (join != null)
            input.Join = join;

        if (order != null)
            input.OrderBy = order;

        var entities = Context.Select<T>(input);
        return entities;
    }
}
