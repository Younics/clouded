using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Input;
using MudBlazor;

namespace Clouded.Admin.Provider.Contexts;

public class AdminContext(Connection connection, ISnackbar snackbar) : Context(connection)
{
    public new void CreateTable(TableInput input, bool ifNotExists = false) =>
        ExecuteFunc(() => base.CreateTable(input, ifNotExists));

    public new IEnumerable<TableResult> GetTables(string schema) =>
        ExecuteFunc(() => base.GetTables(schema));

    public new IEnumerable<ColumnResult> GetColumns(string schema, string table) =>
        ExecuteFunc(() => base.GetColumns(schema, table));

    public new IEnumerable<RelationResult> GetInsideRelations(string schema, string table) =>
        ExecuteFunc(() => base.GetInsideRelations(schema, table));

    public new IEnumerable<RelationResult> GetOutsideRelations(string schema, string table) =>
        ExecuteFunc(() => base.GetOutsideRelations(schema, table));

    public new long Count(SelectInput input) => ExecuteFunc(() => base.Count(input));

    public new bool Exists(SelectInput input) => ExecuteFunc(() => base.Exists(input));

    public new IEnumerable<DataSourceDictionary> Select(SelectInput input) =>
        ExecuteFunc(() => base.Select<DataSourceDictionary>(input));

    public new IEnumerable<T> Select<T>(SelectInput input)
        where T : DataSourceDictionary, new() => ExecuteFunc(() => base.Select<T>(input));

    public new DataSourceDictionary SelectSingle(SelectInput input) =>
        ExecuteFunc(() => base.SelectSingle<DataSourceDictionary>(input));

    public new T SelectSingle<T>(SelectInput input)
        where T : DataSourceDictionary, new() => ExecuteFunc(() => base.SelectSingle<T>(input));

    public new void Create(CreateInput input) =>
        ExecuteFunc(() => base.Create<DataSourceDictionary>(input));

    public new T Create<T>(CreateInput input)
        where T : DataSourceDictionary, new() => ExecuteFunc(() => base.Create<T>(input));

    public new void Update(UpdateInput input) =>
        ExecuteFunc(() => base.Update<DataSourceDictionary>(input));

    public new T Update<T>(UpdateInput input)
        where T : DataSourceDictionary, new() => ExecuteFunc(() => base.Update<T>(input));

    public new void Delete(DeleteInput input) => ExecuteFunc(() => base.Delete(input));

    private void ExecuteFunc(Action func)
    {
        try
        {
            func.Invoke();
        }
        catch (Exception e)
        {
            Console.Write(e);
            snackbar.Add("Something went wrong. Try it later.", Severity.Error);
            throw;
        }
    }

    private TResult ExecuteFunc<TResult>(Func<TResult> func)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception e)
        {
            Console.Write(e);
            snackbar.Add("Something went wrong. Try it later.", Severity.Error);
            throw;
        }

        return default;
    }
}
