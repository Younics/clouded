// See https://aka.ms/new-console-template for more information

using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;

var context = new Context(
    new Connection
    {
        Type = DatabaseType.PostgreSQL,
        Server = "localhost",
        Port = 3306,
        Username = "root",
        Password = "secret",
        Database = "clouded_test"
    }
);

var asd = context.GetTables("public");
var dsa = context.GetColumns("public", "users");

Console.WriteLine("Clouded!");
