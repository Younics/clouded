using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Api;

public class Context
{
    private readonly IContext _context;

    public Context(Connection connection)
    {
        switch (connection.Type)
        {
            case DatabaseType.PostgreSQL:
                _context = new Postgres.Context(connection);
                break;
            case DatabaseType.MySQL:
                _context = new Mysql.Context(connection);
                break;
            default:
                throw new NullReferenceException("Connection wasn't specified.");
        }
    }

    public void TestConnection() => _context.TestConnection();

    public Task<string> GetDatabaseSchema() => _context.GetDatabaseSchema();

    public IEnumerable<string> GetSchema() => _context.GetSchema();

    public void CreateTable(TableInput input, bool ifNotExists = false) =>
        _context.CreateTable(input, ifNotExists);

    public IEnumerable<TableResult> GetTables(string schema) => _context.GetTables(schema);

    public IEnumerable<ColumnResult> GetColumns(string schema, string table) =>
        _context.GetColumns(schema, table);

    public IEnumerable<RelationResult> GetInsideRelations(string schema, string table) =>
        _context.GetInsideRelations(schema, table);

    public IEnumerable<RelationResult> GetOutsideRelations(string schema, string table) =>
        _context.GetOutsideRelations(schema, table);

    public long Count(SelectInput input) => _context.Count(input);

    public bool Exists(SelectInput input) => _context.Exists(input);

    public IEnumerable<DataSourceDictionary> Select(SelectInput input) =>
        _context.Select<DataSourceDictionary>(input);

    public IEnumerable<T> Select<T>(SelectInput input)
        where T : DataSourceDictionary, new() => _context.Select<T>(input);

    public DataSourceDictionary SelectSingle(SelectInput input) =>
        _context.SelectSingle<DataSourceDictionary>(input);

    public T SelectSingle<T>(SelectInput input)
        where T : DataSourceDictionary, new() => _context.SelectSingle<T>(input);

    public void Create(CreateInput input) => _context.Create<DataSourceDictionary>(input);

    public T Create<T>(CreateInput input)
        where T : DataSourceDictionary, new() => _context.Create<T>(input);

    public void Update(UpdateInput input) => _context.Update<DataSourceDictionary>(input);

    public T Update<T>(UpdateInput input)
        where T : DataSourceDictionary, new() => _context.Update<T>(input);

    public void Delete(DeleteInput input) => _context.Delete(input);

    public bool CheckIfConnectionIsReadonly() => _context.CheckIfConnectionIsReadonly();
}
