using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Shared.Pagination;
using Clouded.Core.DataSource.Shared;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IDataSource<T>
    where T : BaseDictionary
{
    public BaseEntityIdentityOptions? GetBaseEntityIdentityOptions();
    public IEnumerable<ColumnResult> GetEntityColumns();

    public Paginated<T> EntitiesPaginated(
        int page,
        int size,
        object? search = null,
        (IEnumerable<ColumnResult>, Dictionary<string, object?>)? filter = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );

    public IEnumerable<T> Entities(
        object? search = null,
        string? orderByColumn = null,
        bool orderByDescending = false
    );

    public T EntityCreate(T data, DataSourceDictionary? supportData = null);

    public T EntityUpdate(object? id, T data, DataSourceDictionary? supportData = null);

    public void EntityDelete(object? id);

    public bool EntityExists(T data, bool conditionsJoinedWithOr = false);

    public T EntityFindById(object? id);

    public T EntityFindByIdentity(object? identity);

    public IEnumerable<T> EntitySearch(object? value);

    public void EntityMetaDataUpdate(object? entityId, Dictionary<string, object?> data);

    public Dictionary<string, object?> EntityMetaData(object? entityId);

    public T EntitySupportData(object? entityId);

    public T EntitySupportTable(object? entityId);

    public void TableVerification();
    public void SupportTableSetup();

    public ColumnResult GetColumnId(string schema, string table, string column);
}
