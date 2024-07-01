using System.Net;

namespace Clouded.Results.Exceptions;

public class ColumnNotFoundException : CloudedException
{
    public ColumnNotFoundException(string column)
        : base(
            $"Column '{column}' not found!",
            "general.column_not_found",
            (int)HttpStatusCode.NotFound
        ) { }

    public ColumnNotFoundException(string column, string table)
        : base(
            $"Column '{column}' not found in the table '{table}'!",
            "general.column_not_found",
            (int)HttpStatusCode.NotFound
        ) { }
}
