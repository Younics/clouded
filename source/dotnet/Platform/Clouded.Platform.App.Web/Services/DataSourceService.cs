using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Database.Contexts.Base;
using Clouded.Platform.Database.Entities.Clouded;
using Clouded.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services;

public class DataSourceService(
    ApplicationOptions options,
    IDbContextResolver dbContextResolver,
    NavigationManager navigationManager
) : IDataSourceService
{
    protected readonly DatabaseOptions DatabaseOptions = options.Clouded.Database;

    public Context LoadDatasourceContext(
        DataSourceEntity dataSourceEntity,
        Dictionary<long, IEnumerable<string>?> schemas,
        Dictionary<string, IEnumerable<TableResult>?> tables,
        Dictionary<string, IEnumerable<ColumnResult>?> columns
    )
    {
        var datasourceContext = new Context(
            new Connection
            {
                Type = dataSourceEntity.Configuration.Type,
                Server = dataSourceEntity.Configuration.Server.Decrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                ),
                Port = dataSourceEntity.Configuration.Port,
                Username = dataSourceEntity.Configuration.Username.Decrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                ),
                Password = dataSourceEntity.Configuration.Password.Decrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                ),
                Database = dataSourceEntity.Configuration.Database.Decrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                )
            }
        );

        datasourceContext.TestConnection();

        schemas[dataSourceEntity.Id] = datasourceContext.GetSchema().ToList().Order();

        foreach (var schema in schemas[dataSourceEntity.Id])
        {
            tables[GetTablesKey(dataSourceEntity.Id, schema)] = GetSchemaTables(
                datasourceContext,
                schema
            );
            foreach (var tableResult in tables[GetTablesKey(dataSourceEntity.Id, schema)])
            {
                columns[
                    GetColumnsKey(
                        dataSourceEntity.Id,
                        tableResult.SchemaName,
                        tableResult.TableName
                    )
                ] = GetTableColumns(datasourceContext, tableResult);
            }
        }

        return datasourceContext;
    }

    public async Task<DataSourceEntity?> CreateAsync(
        DataSourceInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var dataSource = new DataSourceEntity
        {
            Name = input.Name!,
            Configuration = new DataSourceConfigurationEntity
            {
                Type = input.Type!.Value,
                Server = input.Server!.Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey),
                Port = input.Port!.Value,
                Username = input.Username!.Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey),
                Password = input.Password!.Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey),
                Database = input.Database!.Encrypt(DatabaseOptions.CloudedConnection.EncryptionKey)
            },
            ProjectId = input.ProjectId!.Value,
        };

        await context.CreateAsync(dataSource, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return dataSource;
    }

    public async Task<DataSourceEntity> UpdateAsync(
        DataSourceInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        var dataSource = await context.UpdateAsync<DataSourceEntity>(
            input.Id!.Value,
            entity =>
            {
                entity.Name = input.Name!;
                entity.Configuration.Type = input.Type!.Value;
                entity.Configuration.Server = input.Server!.Encrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                );
                entity.Configuration.Port = input.Port!.Value;
                entity.Configuration.Username = input.Username!.Encrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                );
                entity.Configuration.Password = input.Password!.Encrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                );
                entity.Configuration.Database = input.Database!.Encrypt(
                    DatabaseOptions.CloudedConnection.EncryptionKey
                );
                entity.ProjectId = input.ProjectId!.Value;
            },
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return dataSource;
    }

    public async Task DeleteAsync(
        DataSourceEntity entity,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    )
    {
        context ??= dbContextResolver.UseScopedContext<CloudedDbContext>();
        var transaction = await context.TransactionStartAsync(cancellationToken);

        context.Delete(entity);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public String GetEditRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/datasources/{modelId}/edit";
    }

    public String GetDetailRoute(string projectId, long? modelId)
    {
        return $"/projects/{projectId}/datasources/{modelId}";
    }

    public String GetListRoute(string projectId)
    {
        return $"/projects/{projectId}/datasources";
    }

    public static string GetTablesKey(long datasourceId, string schemaName)
    {
        return $"{datasourceId}-{schemaName}";
    }

    public static string GetColumnsKey(long datasourceId, string schemaName, string tableName)
    {
        return $"{datasourceId}-{schemaName}-{tableName}";
    }

    public static IOrderedEnumerable<TableResult> GetSchemaTables(
        Context datasourceContext,
        string schema
    )
    {
        return datasourceContext.GetTables(schema).ToList().OrderBy(x => x.TableName);
    }

    public static List<ColumnResult> GetTableColumns(Context datasourceContext, TableResult table)
    {
        return datasourceContext
            .GetColumns(table.SchemaName, table.TableName)
            .OrderByDescending(x => x.IsPrimary)
            .ThenByDescending(x => x.IsForeignKey)
            .ToList();
    }
}
