namespace Clouded.Core.DataSource.Shared.Extensions;

public static class ConnectionExtensions
{
    public static string PostgresConnectionString(this Connection connection) =>
        $"Server={connection.Server};"
        + $"Port={connection.Port};"
        + $"Database={connection.Database};"
        + $"User Id={connection.Username};"
        + $"Password={connection.Password}";

    public static string MysqlConnectionString(this Connection connection) =>
        $"Server={connection.Server};"
        + $"Port={connection.Port};"
        + $"User Id={connection.Username};"
        + $"Password={connection.Password}";
}
