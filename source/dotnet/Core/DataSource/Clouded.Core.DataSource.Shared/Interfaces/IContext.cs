using Clouded.Core.DataSource.Shared.Input;

namespace Clouded.Core.DataSource.Shared.Interfaces;

public interface IContext
{
    public void TestConnection();
    public Task<string> GetDatabaseSchema();

    public IEnumerable<string> GetSchema();

    public void CreateTable(TableInput input, bool ifNotExists = false);

    public IEnumerable<TableResult> GetTables(string schema);

    public IEnumerable<ColumnResult> GetColumns(string schema, string table);

    public IEnumerable<RelationResult> GetInsideRelations(string schema, string table);

    public IEnumerable<RelationResult> GetOutsideRelations(string schema, string table);

    public long Count(SelectInput input);

    public bool Exists(SelectInput input);

    public IEnumerable<T> Select<T>(SelectInput input)
        where T : DataSourceDictionary, new();
    public T SelectSingle<T>(SelectInput input)
        where T : DataSourceDictionary, new();

    public T Create<T>(CreateInput input)
        where T : DataSourceDictionary, new();

    public T Update<T>(UpdateInput input)
        where T : DataSourceDictionary, new();

    public void Delete(DeleteInput input);
    public bool CheckIfConnectionIsReadonly();
}
