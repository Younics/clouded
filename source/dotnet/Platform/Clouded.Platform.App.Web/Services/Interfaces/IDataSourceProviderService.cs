using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;
using Clouded.Platform.App.Web.Dtos;
using Clouded.Platform.Database.Entities.Clouded;
using Microsoft.EntityFrameworkCore;

namespace Clouded.Platform.App.Web.Services.Interfaces;

public interface IDataSourceService : IBaseService
{
    public Context LoadDatasourceContext(
        DataSourceEntity dataSourceEntity,
        Dictionary<long, IEnumerable<string>?> schemas,
        Dictionary<string, IEnumerable<TableResult>?> tables,
        Dictionary<string, IEnumerable<ColumnResult>?> columns
    );

    public Task<DataSourceEntity?> CreateAsync(
        DataSourceInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task<DataSourceEntity> UpdateAsync(
        DataSourceInput input,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public Task DeleteAsync(
        DataSourceEntity entity,
        DbContext? context = default,
        CancellationToken cancellationToken = default
    );

    public String GetEditRoute(string projectId, long? modelId);
    public String GetDetailRoute(string projectId, long? modelId);
    public String GetListRoute(string projectId);
}
