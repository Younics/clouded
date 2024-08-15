using System.Data;
using System.Data.Common;
using System.Text;
using System.Web;
using CliWrap;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Core.DataSource.Shared.Interfaces;
using Clouded.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Clouded.Core.DataSource.Postgres;

public class Context(Connection connection) : DbContext, IContext
{
    private readonly string _connectionString = connection.PostgresConnectionString();
    private readonly string[] _prohibitedSchemas = { "information_schema", "pg_catalog", "pg_toast" };

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(_connectionString);

    public void TestConnection()
    {
        var connection = Database.GetDbConnection();
        if (connection.State == ConnectionState.Closed)
            Database.OpenConnection();

        Database.CloseConnection();
    }

    public async Task<string> GetDatabaseSchema()
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        // TODO: Watch out for command injection, sanitize input before using
        // TODO: pg_dump must be installed before running this command
        var dumpResult = await Cli.Wrap("pg_dump")
            .WithArguments(
                new[]
                {
                    "-s",
                    $"postgres://{HttpUtility.UrlEncode(connection.Username)}:{connection.Password}@{HttpUtility.UrlEncode(connection.Server)}:{connection.Port}/{connection.Database}"
                }
            )
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync();

        if (dumpResult.ExitCode != 0)
        {
            // TODO: Throw customized exception with parsed error from buffer
            return string.Empty;
        }

        return stdOutBuffer.ToString();
    }

    public IEnumerable<string> GetSchema()
    {
        var schema = Select<DataSourceDictionary>(
            new SelectInput
            {
                Schema = "information_schema",
                Table = "schemata",
                Alias = "schema",
                SelectedColumns = new SelectColDesc[] { new() { ColJoin = new[] { "schema", "schema_name" } } }
            }
        );

        return schema
            .Select(row => Convert.ToString(row["schema_name"]) ?? string.Empty)
            .Where(x => !_prohibitedSchemas.Contains(x));
    }

    public void CreateTable(TableInput input, bool ifNotExists = false)
    {
        var columns = input.Columns
            .Select(column =>
            {
                var sql =
                    $""" "{column.Name}" {column.TypeRaw ?? ResolveColumnType(column.Type)} {(column.NotNull ? "not null" : string.Empty)} """;
                var reference = column.Reference;
                var primaryKey = column.PrimaryKey;
                var uniqueKey = column.UniqueKey;

                if (reference != null)
                {
                    sql +=
                        $"""CONSTRAINT "FK_{reference.Name}" """
                        + $"""REFERENCES "{reference.TargetSchema}"."{reference.TargetTable}" ("{reference.TargetColumn}") """;

                    if (reference.OnUpdate != ActionType.NoAction)
                        sql += $"ON UPDATE {ResolveActionType(reference.OnUpdate)} ";

                    if (reference.OnDelete != ActionType.NoAction)
                        sql += $"ON DELETE {ResolveActionType(reference.OnDelete)} ";
                }

                if (primaryKey != null)
                {
                    var primaryKeys = primaryKey.Columns.Select(key => $""" "{key}" """);

                    sql +=
                        $"""CONSTRAINT "PK_{primaryKey.Name}" """
                        + $"PRIMARY KEY ({string.Join(',', primaryKeys)})";
                }

                if (uniqueKey != null)
                {
                    var uniqueKeys = uniqueKey.Columns.Select(key => $""" "{key}" """);

                    sql +=
                        $"""CONSTRAINT "UK_{uniqueKey.Name}" """
                        + $"UNIQUE ({string.Join(',', uniqueKeys)})";
                }

                return sql;
            })
            .ToList();

        // TODO REFERENCES

        var tablePrimaryKeys = input.PrimaryKeys
            .Select(constraint =>
            {
                var primaryKeys = constraint.Columns.Select(key => $""" "{key}" """);

                return $"""CONSTRAINT "PK_{constraint.Name}" """
                       + $"""PRIMARY KEY ({string.Join(',', primaryKeys)})""";
            })
            .ToList();

        var tableUniqueKeys = input.UniqueKeys
            .Select(constraint =>
            {
                var uniqueKeys = constraint.Columns.Select(key => $""" "{key}" """);

                return $"""CONSTRAINT "UK_{constraint.Name}" """
                       + $"""UNIQUE ({string.Join(',', uniqueKeys)})""";
            })
            .ToList();

        var sql =
            $"""CREATE TABLE {(ifNotExists ? "IF NOT EXISTS" : string.Empty)} "{input.Schema}"."{input.Name}" """
            + "(";

        if (columns.Any())
            sql += $"{string.Join(',', columns)} \n";

        if (tablePrimaryKeys.Any())
            sql += $",{string.Join("\n", tablePrimaryKeys)} \n";

        if (tableUniqueKeys.Any())
            sql += $",{string.Join("\n", tableUniqueKeys)}";

        sql += ")";

        Execute(sql);
    }

    public IEnumerable<TableResult> GetTables(string schema)
    {
        var tables = Select<DataSourceDictionary>(
            new SelectInput
            {
                Schema = "information_schema",
                Table = "tables",
                Alias = "tables",
                Where = new ConditionValueInput
                {
                    Alias = "tables", Column = "table_schema", Operator = EOperator.Equals, Value = schema
                }
            }
        );

        return tables.Select(
            row =>
                new TableResult
                {
                    SchemaName = Convert.ToString(row["table_schema"]) ?? string.Empty,
                    TableName = Convert.ToString(row["table_name"]) ?? string.Empty
                }
        );
    }

    public IEnumerable<ColumnResult> GetColumns(string schema, string table)
    {
        var columns = Select<DataSourceDictionary>(
            new SelectInput
            {
                Schema = "information_schema",
                Table = "columns",
                Alias = "columns",
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = "columns",
                            Column = "table_schema",
                            Operator = EOperator.Equals,
                            Value = schema
                        },
                        new ConditionValueInput
                        {
                            Alias = "columns",
                            Column = "table_name",
                            Operator = EOperator.Equals,
                            Value = table
                        }
                    }
                }
            }
        );

        var primaryColumns = Query<DataSourceDictionary>(
                $"""
                     SELECT "attribute"."attname", "attribute"."attidentity"
                     FROM "pg_catalog"."pg_index" "index"
                     JOIN "pg_catalog"."pg_attribute" "attribute" ON "attribute"."attrelid" = "index"."indrelid" AND "attribute"."attnum" = ANY("index"."indkey")
                     WHERE "index"."indrelid" = '"{schema}"."{table}"'::regclass AND "index"."indisprimary" 
                 """
            )
            .Select(
                x =>
                (
                    (string?)x.Values.First(),
                    ((char?)x.Values.Last()).ToString() == "d"
                    || ((char?)x.Values.Last()).ToString() == "a" //a = generated always, d = generated by default
                )
            ).ToList();

        var insideRelations = GetInsideRelations(schema, table);
        var relationResults = insideRelations as RelationResult[] ?? insideRelations.ToArray();

        return columns.Select(row =>
        {
            var columnName = Convert.ToString(row["column_name"]) ?? string.Empty;
            var columnTypeRaw = Convert.ToString(row["data_type"]);

            return new ColumnResult
            {
                SchemaName = Convert.ToString(row["table_schema"]) ?? string.Empty,
                TableName = Convert.ToString(row["table_name"]) ?? string.Empty,
                Name = columnName,
                TypeRaw = columnTypeRaw,
                Type = columnTypeRaw switch
                {
                    "boolean" => EColumnType.Boolean,
                    "char" or "character" => EColumnType.Char,
                    "varchar" or "character varying" => EColumnType.Varchar,
                    "cidr" => EColumnType.Varchar,
                    "inet" => EColumnType.Varchar,
                    "text" => EColumnType.Text,
                    "json" => EColumnType.Text,
                    "macaddr" => EColumnType.Text,
                    "macaddr8" => EColumnType.Text,
                    "integer" or "smallint" or "smallserial" or "serial" => EColumnType.Int,
                    "bigint" => EColumnType.Long,
                    "bigserial" => EColumnType.Long,
                    "float" or "double" or "double precision" or "real" => EColumnType.Double,
                    "decimal" or "money" => EColumnType.Decimal,
                    "numeric" => EColumnType.Decimal,
                    "time" => EColumnType.Time,
                    "date" => EColumnType.Date,
                    "bytea" => EColumnType.Bytea,
                    "uuid" => EColumnType.Varchar,
                    "timestamp"
                        or "timestamp without time zone"
                        or "timestamp with time zone"
                        => EColumnType.DateTime,
                    _ => EColumnType.Unsupported
                },
                Position = Convert.ToInt32(row["ordinal_position"]),
                IsNullable = Convert.ToString(row["is_nullable"]) == "YES",
                MaxLength =
                    row["character_maximum_length"] != null
                        ? Convert.ToInt16(row["character_maximum_length"])
                        : null,
                IsPrimary = primaryColumns.Any(primaryColumn => primaryColumn.Item1 == columnName),
                IsAutoIncrement = primaryColumns.Any(
                    primaryColumn => primaryColumn.Item1 == columnName && primaryColumn.Item2
                ),
                IsGenerated = columnTypeRaw == "uuid" || primaryColumns.Any(
                    primaryColumn => primaryColumn.Item1 == columnName && primaryColumn.Item2
                ),
                IsForeignKey = relationResults.Any(
                    x => x.Column == columnName || x.TargetColumn == columnName
                ),
                InsideRelation = relationResults.FirstOrDefault(
                    x => x.Column == columnName || x.TargetColumn == columnName
                )
            };
        });
    }

    public IEnumerable<RelationResult> GetInsideRelations(string schema, string table)
    {
        try
        {
            var foreignKeys = Query<DataSourceDictionary>(
                $"""
                     SELECT from_class.relnamespace::regnamespace::text from_schema,
                            to_class.relnamespace::regnamespace::text to_schema,
                            conrelid::regclass::text AS from_table,
                            confrelid::regclass::text AS to_table,
                            from_attribute.attname AS from_attribute,
                            to_attribute.attname AS to_attribute,
                            from_attribute.attnotnull AS from_attribute_not_null,
                            to_attribute.attnotnull AS to_attribute_not_null,
                             index_class.indisunique as cond_is_unique
                     FROM pg_constraint pgc
                     LEFT JOIN pg_attribute from_attribute ON from_attribute.attrelid = pgc.conrelid AND from_attribute.attnum = ANY(pgc.conkey)
                     LEFT JOIN pg_class from_class ON from_attribute.attrelid = from_class.oid
                     LEFT JOIN pg_attribute to_attribute ON to_attribute.attrelid = pgc.confrelid AND to_attribute.attnum = ANY(pgc.confkey)
                     LEFT JOIN pg_class to_class ON to_attribute.attrelid = to_class.oid
                     LEFT JOIN pg_index index_class ON conindid = index_class.indexrelid
                     WHERE contype = 'f' AND connamespace = '{schema}'::regnamespace AND conrelid::regclass = '{table}'::regclass
                 """
            );

            return foreignKeys.Select(
                row =>
                    new RelationResult
                    {
                        Schema = Convert.ToString(row["from_schema"]) ?? string.Empty,
                        Table = Convert.ToString(row["from_table"]) ?? string.Empty,
                        Column = Convert.ToString(row["from_attribute"]) ?? string.Empty,
                        TargetSchema = Convert.ToString(row["to_schema"]) ?? string.Empty,
                        TargetTable = Convert.ToString(row["to_table"]) ?? string.Empty,
                        TargetColumn = Convert.ToString(row["to_attribute"]) ?? string.Empty,
                        ColumnNotNull = Convert.ToBoolean(row["from_attribute_not_null"]),
                        TargetColumnNotNull = Convert.ToBoolean(row["to_attribute_not_null"]),
                        IsUnique = Convert.ToBoolean(row["cond_is_unique"]),
                    }
            );
        }
        catch (Exception e)
        {
            Console.Write(e);
            return new List<RelationResult>();
        }
    }

    public IEnumerable<RelationResult> GetOutsideRelations(string schema, string table)
    {
        try
        {
            var foreignKeys = Query<DataSourceDictionary>(
                $"""
                     SELECT from_class.relnamespace::regnamespace::text from_schema,
                            to_class.relnamespace::regnamespace::text to_schema,
                            confrelid::regclass::text AS from_table,
                            conrelid::regclass::text AS to_table,
                            from_attribute.attname AS from_attribute,
                            to_attribute.attname AS to_attribute,
                            from_attribute.attnotnull AS from_attribute_not_null,
                            to_attribute.attnotnull AS to_attribute_not_null,
                            coalesce(unique_indexes.record_exists, false) AS cond_is_unique
                     FROM pg_constraint pgc
                     LEFT JOIN pg_attribute from_attribute ON from_attribute.attrelid = pgc.confrelid AND from_attribute.attnum = ANY(pgc.confkey)
                     LEFT JOIN pg_class from_class ON from_attribute.attrelid = from_class.oid
                     LEFT JOIN pg_attribute to_attribute ON to_attribute.attrelid = pgc.conrelid AND to_attribute.attnum = ANY(pgc.conkey)
                     LEFT JOIN pg_class to_class ON to_attribute.attrelid = to_class.oid
                     LEFT JOIN (select t.relnamespace::regnamespace::text                              as table_schema,
                                      t.relname                                                       as table_name,
                                      i.relname                                                       as index_name,
                                      a.attname                                                       as column_name,
                                      (SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS bool)) as record_exists
                               from pg_class t,
                                    pg_class i,
                                    pg_index ix,
                                    pg_attribute a
                               where t.oid = ix.indrelid
                                 and i.oid = ix.indexrelid
                                 and a.attrelid = t.oid
                                 and a.attnum = ANY (ix.indkey)
                                 and t.relkind = 'r'
                                 and ix.indisunique) unique_indexes
                              on to_class.relnamespace::regnamespace::text = unique_indexes.table_schema and
                                 conrelid::regclass::text = unique_indexes.table_name and
                                 to_attribute.attname = unique_indexes.column_name
                     WHERE contype = 'f' AND connamespace = '{schema}'::regnamespace AND confrelid::regclass = '{table}'::regclass
                 """
            );

            return foreignKeys.Select(
                row =>
                    new RelationResult
                    {
                        Schema = Convert.ToString(row["from_schema"]) ?? string.Empty,
                        Table = Convert.ToString(row["from_table"]) ?? string.Empty,
                        Column = Convert.ToString(row["from_attribute"]) ?? string.Empty,
                        TargetSchema = Convert.ToString(row["to_schema"]) ?? string.Empty,
                        TargetTable = Convert.ToString(row["to_table"]) ?? string.Empty,
                        TargetColumn = Convert.ToString(row["to_attribute"]) ?? string.Empty,
                        ColumnNotNull = Convert.ToBoolean(row["from_attribute_not_null"]),
                        TargetColumnNotNull = Convert.ToBoolean(row["to_attribute_not_null"]),
                        IsUnique = Convert.ToBoolean(row["cond_is_unique"]),
                    }
            );
        }
        catch (Exception e)
        {
            Console.Write(e);
            return new List<RelationResult>();
        }
    }

    public long Count(SelectInput input)
    {
        var select = ResolveSelect(input, true);
        var query = QuerySingle<DataSourceDictionary>(select.Sql, select.Parameters);

        return (long)(query.GetValueOrDefault("count") ?? 0L);
    }

    public bool Exists(SelectInput input)
    {
        var select = ResolveSelect(input);
        return (bool)
            QuerySingle<DataSourceDictionary>($"SELECT EXISTS({select.Sql})", select.Parameters)[
                "exists"
            ]!;
    }

    public IEnumerable<T> Select<T>(SelectInput input)
        where T : DataSourceDictionary, new()
    {
        var select = ResolveSelect(input);
        return Query<T>(select.Sql, select.Parameters);
    }

    public T SelectSingle<T>(SelectInput input)
        where T : DataSourceDictionary, new()
    {
        var select = ResolveSelect(input);
        return QuerySingle<T>(select.Sql, select.Parameters);
    }

    public T Create<T>(CreateInput input)
        where T : DataSourceDictionary, new()
    {
        var columns = string.Join(',', input.Data.Select(column => $""" "{column.Key}" """));

        var values = string.Join(',', input.Data.Select(column => $":{column.Key}"));

        var sql =
            $"""INSERT INTO "{input.Schema}"."{input.Table}" """
            + "\n"
            + $"""({columns})"""
            + "\n"
            + $"""VALUES ({values})"""
            + "\n"
            + "ON CONFLICT DO NOTHING \n";

        if (input.ReturnColumnsRaw != null)
            sql += $"""RETURNING {input.ReturnColumnsRaw}""";

        return QuerySingle<T>(sql, input.Data);
    }

    public T Update<T>(UpdateInput input)
        where T : DataSourceDictionary, new()
    {
        var parameters = ResolveConditionParameters(input.Where)
            .Concat(input.Data)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        var columns = string.Join(
            ',',
            input.Data.Select(column => $""" "{column.Key}" = :{column.Key} """)
        );

        var sql =
            $"""UPDATE "{input.Schema}"."{input.Table}" "{input.Alias}" """
            + "\n"
            + $"""SET {columns}"""
            + "\n";

        if (input.Where != null)
            sql += $""" WHERE {ResolveConditionSql(input.Where)} """ + "\n";

        if (input.ReturnColumnsRaw != null)
            sql += $"""RETURNING {input.ReturnColumnsRaw}""";

        return QuerySingle<T>(sql, parameters);
    }

    public void Delete(DeleteInput input)
    {
        var parameters = ResolveConditionParameters(input.Where);

        var sql = $"""DELETE FROM "{input.Schema}"."{input.Table}" "{input.Alias}" """ + "\n";

        if (input.Where != null)
            sql += $""" WHERE {ResolveConditionSql(input.Where)} """ + "\n";

        Execute(sql, parameters);
    }

    public bool CheckIfConnectionIsReadonly()
    {
        var uuid = Guid.NewGuid().ToString();
        var tableName = "dummy_table" + uuid;

        try
        {
            CreateTable(
                new TableInput
                {
                    Schema = "public",
                    Name = tableName,
                    Columns = new List<ColumnInput> { new() { Name = "test", Type = EColumnType.Boolean, } }
                }
            );

            Execute($"DROP TABLE \"{tableName}\"");
        }
        catch (Exception e)
        {
            Console.Write(e);
            return true;
        }

        return false;
    }

    private void Execute(
        string sql,
        IEnumerable<KeyValuePair<string, object?>>? parameters = null
    ) => QuerySingle<DataSourceDictionary>(sql, parameters);

    private IEnumerable<T> Query<T>(
        string sql,
        IEnumerable<KeyValuePair<string, object?>>? parameters = null,
        CommandType commandType = CommandType.Text
    )
        where T : DataSourceDictionary, new()
    {
        var connection = Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = commandType;

        if (parameters != null)
            command.Parameters.AddRange(
                parameters
                    .Select(
                        parameter =>
                            parameter.Value is not object[] objects
                                ? new NpgsqlParameter(
                                    parameter.Key,
                                    parameter.Value ?? DBNull.Value
                                )
                                : objects.All(@object => @object is long)
                                    ? new NpgsqlParameter(
                                        parameter.Key,
                                        objects.Cast<long>().ToArray()
                                    )
                                    : objects.All(@object => @object is decimal)
                                        ? new NpgsqlParameter(
                                            parameter.Key,
                                            objects.Cast<decimal>().ToArray()
                                        )
                                        : new NpgsqlParameter(
                                            parameter.Key,
                                            parameter.Value ?? DBNull.Value
                                        )
                    )
                    .ToArray()
            );

        if (connection.State == ConnectionState.Closed)
            Database.OpenConnection();

        var result = new List<T>();
        DbDataReader reader;
        try
        {
            reader = command.ExecuteReader();
        }
        catch (PostgresException e)
        {
            Console.WriteLine("SQL: " + sql);
            throw e;
        }

        while (reader.Read())
        {
            var row = new T();

            for (var columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                var value = reader.GetValue(columnIndex);

                row.Add(reader.GetName(columnIndex), value is not DBNull ? value : null);
            }

            result.Add(row);
        }

        Database.CloseConnection();

        return result;
    }

    private T QuerySingle<T>(
        string sql,
        IEnumerable<KeyValuePair<string, object?>>? parameters = null,
        CommandType commandType = CommandType.Text
    )
        where T : DataSourceDictionary, new()
    {
        var connection = Database.GetDbConnection();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = commandType;

        if (parameters != null)
        {
            command.Parameters.AddRange(
                parameters
                    .Select(
                        parameter =>
                            parameter.Value is not object[] objects
                                ? new NpgsqlParameter(
                                    parameter.Key,
                                    parameter.Value ?? DBNull.Value
                                )
                                : objects.All(@object => @object is long)
                                    ? new NpgsqlParameter(
                                        parameter.Key,
                                        objects.Cast<long>().ToArray()
                                    )
                                    : objects.All(@object => @object is decimal)
                                        ? new NpgsqlParameter(
                                            parameter.Key,
                                            objects.Cast<decimal>().ToArray()
                                        )
                                        : new NpgsqlParameter(
                                            parameter.Key,
                                            parameter.Value ?? DBNull.Value
                                        )
                    )
                    .ToArray()
            );
        }

        if (connection.State == ConnectionState.Closed)
            Database.OpenConnection();

        var result = new T();
        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            for (var columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
            {
                var value = reader.GetValue(columnIndex);

                result.Add(reader.GetName(columnIndex), value is not DBNull ? value : null);
            }
        }

        Database.CloseConnection();

        return result;
    }

    private static string ResolveColumnType(EColumnType? type) =>
        type switch
        {
            EColumnType.Boolean => "boolean",
            EColumnType.Char => "char",
            EColumnType.Varchar => "varchar",
            EColumnType.Text => "text",
            EColumnType.Int => "integer",
            EColumnType.Long => "bigint",
            EColumnType.Double => "float",
            EColumnType.Decimal => "decimal",
            EColumnType.Time => "time",
            EColumnType.Date => "date",
            EColumnType.DateTime => "timestamp",
            EColumnType.SmallSerial => "smallserial",
            EColumnType.Serial => "serial",
            EColumnType.BigSerial => "bigserial",
            EColumnType.Bytea => "bytea",
            _ => throw new NotSupportedException($"Column type '{type}' is not supported yet.")
        };

    private static (string Sql, Dictionary<string, object?> Parameters) ResolveSelect(
        SelectInput input,
        bool useCount = false
    )
    {
        var parameters = ResolveConditionParameters(input.Where)
            .Concat(input.Join.SelectMany(join => ResolveConditionParameters(join.On)))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        var join = input.Join
            .Select(
                join =>
                    $"""LEFT JOIN "{join.Schema}"."{join.Table}" "{join.Alias}" ON {ResolveConditionSql(join.On)} """
            )
            .ToList();

        var groupBy = input.GroupBy.Select(group => $""" "{group.Alias}"."{group.Column}" """);

        var orderBy = input.OrderBy.Select(
            order =>
                $""" "{order.Alias}"."{order.Column}" {(order.Direction == OrderType.Asc ? "ASC" : "DESC")} """
        );

        var sql =
            $""" SELECT {(input.Distinct && !useCount ? "DISTINCT" : "")} {(useCount ? $"COUNT({(input.Distinct && useCount ? "DISTINCT " : "")}{input.SelectedColumnsRaw})" : input.SelectedColumnsRaw)} """
            + "\n"
            + $""" FROM "{input.Schema}"."{input.Table}" "{input.Alias}" """
            + "\n";

        if (join.Any())
            sql += $""" {string.Join("\n", join)} """ + "\n";

        if (input.Where != null)
        {
            var whereConditionSql = ResolveConditionSql(input.Where);

            if (
                !string.IsNullOrEmpty(whereConditionSql)
                && whereConditionSql != "()"
                && whereConditionSql != "( AND )"
                && whereConditionSql != "( OR )"
            )
            {
                sql += $""" WHERE {whereConditionSql} """ + "\n";
            }
        }

        if (input.GroupBy.Any())
            sql += $""" GROUP BY {string.Join(",", groupBy)} """ + "\n";

        if (input.OrderBy.Any())
            sql += $""" ORDER BY {string.Join("\n", orderBy)} """ + "\n";

        if (input.Offset != null)
            sql += $""" OFFSET {input.Offset} """;

        if (input.Limit != null)
            sql += $""" LIMIT {input.Limit} """;

        return (Sql: sql, Parameters: parameters);
    }

    private static string ResolveConditionSql(ICondition condition)
    {
        var sql = string.Empty;

        switch (condition)
        {
            case DefaultConditionInput:
                sql = "( 1 = 1 )";

                break;
            case ConditionAndInput conditionAnd:
                if (conditionAnd.Conditions.Count() > 1)
                {
                    sql =
                        "("
                        + string.Join(" AND ", conditionAnd.Conditions.Select(ResolveConditionSql))
                        + ")";
                }
                else if (conditionAnd.Conditions.Any())
                {
                    sql =
                        "("
                        + string.Join("", conditionAnd.Conditions.Select(ResolveConditionSql))
                        + ")";
                }
                else
                {
                    sql = "( 1 = 1 )";
                }

                break;
            case ConditionOrInput conditionOr:
                if (conditionOr.Conditions.Count() > 1)
                {
                    sql =
                        "("
                        + string.Join(" OR ", conditionOr.Conditions.Select(ResolveConditionSql))
                        + ")";
                }
                else if (conditionOr.Conditions.Any())
                {
                    sql =
                        "("
                        + string.Join("", conditionOr.Conditions.Select(ResolveConditionSql))
                        + ")";
                }
                else
                {
                    sql = "( 1 = 1 )";
                }

                break;
            case ConditionValueInput conditionValue:
                sql =
                    $""" "{conditionValue.Alias}"."{conditionValue.Column}" """
                    + $""" {ResolveOperator(conditionValue.Operator, conditionValue.Mode)}  """;

                sql += conditionValue.Operator switch
                {
                    EOperator.Contains => $"CONCAT('%',:{conditionValue.Id},'%')",
                    EOperator.StartsWith => $"CONCAT(:{conditionValue.Id},'%')",
                    EOperator.EndsWith => $"CONCAT('%',:{conditionValue.Id})",
                    EOperator.In => $"ANY(:{conditionValue.Id})",
                    EOperator.NotIn => $"ALL(:{conditionValue.Id})",
                    EOperator.IsNull => "",
                    EOperator.IsNotNull => "",
                    _ => $":{conditionValue.Id}"
                };
                break;
            case ConditionColumnInput conditionColumn:
                sql =
                    $""" "{conditionColumn.Alias}"."{conditionColumn.Column}" """
                    + $""" {ResolveOperator(conditionColumn.Operator, conditionColumn.Mode)}  """;

                sql += conditionColumn.Operator switch
                {
                    EOperator.Contains
                        => $""" CONCAT('%',"{conditionColumn.TargetAlias}"."{conditionColumn.TargetColumn}",'%') """,
                    EOperator.StartsWith
                        => $""" CONCAT("{conditionColumn.TargetAlias}"."{conditionColumn.TargetColumn}",'%') """,
                    EOperator.EndsWith
                        => $""" CONCAT('%',"{conditionColumn.TargetAlias}"."{conditionColumn.TargetColumn}") """,
                    EOperator.In
                        or EOperator.NotIn
                        => $""" ("{conditionColumn.TargetAlias}"."{conditionColumn.TargetColumn}") """,
                    _ => $""" "{conditionColumn.TargetAlias}"."{conditionColumn.TargetColumn}" """
                };
                break;
        }

        return sql;
    }

    private static Dictionary<string, object?> ResolveConditionParameters(ICondition? condition)
    {
        var parameters = new Dictionary<string, object?>();

        switch (condition)
        {
            case ConditionAndInput conditionAnd:
                parameters = conditionAnd.Conditions.Aggregate(
                    parameters,
                    (current, cond) =>
                        current
                            .Concat(ResolveConditionParameters(cond))
                            .ToDictionary(pair => pair.Key, pair => pair.Value)
                );
                break;
            case ConditionOrInput conditionOr:
                parameters = conditionOr.Conditions.Aggregate(
                    parameters,
                    (current, cond) =>
                        current
                            .Concat(ResolveConditionParameters(cond))
                            .ToDictionary(pair => pair.Key, pair => pair.Value)
                );
                break;
            case ConditionValueInput conditionValue:
                if (conditionValue.Operator is EOperator.IsNull or EOperator.IsNotNull)
                {
                    break;
                }

                parameters.Add(conditionValue.Id, conditionValue.Value);
                break;
        }

        return parameters;
    }

    private static string ResolveOperator(EOperator @operator, EMode mode = EMode.Sensitive) =>
        @operator switch
        {
            EOperator.Equals => mode == EMode.Sensitive ? "=" : "ILIKE",
            EOperator.NotEquals => mode == EMode.Sensitive ? "!=" : "NOT ILIKE",
            EOperator.GreaterThan => ">",
            EOperator.GreaterThanEquals => ">=",
            EOperator.LessThan => "<",
            EOperator.LessThanEquals => "<=",
            EOperator.In => "=",
            EOperator.NotIn => "!=",
            EOperator.IsNull => "IS NULL",
            EOperator.IsNotNull => "IS NOT NULL",
            EOperator.Contains
                or EOperator.StartsWith
                or EOperator.EndsWith
                => mode == EMode.Sensitive ? "LIKE" : "ILIKE",
            _ => throw new InvalidConstraintException()
        };

    private static string ResolveActionType(ActionType action) =>
        action switch
        {
            ActionType.Restrict => "restrict",
            ActionType.SetNull => "set_null",
            ActionType.SetDefault => "set_default",
            ActionType.Cascade => "cascade",
            _ => "no_action"
        };
}
