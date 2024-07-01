namespace Harbor.Library;

public class HarborClient(string server, string username, string password)
    : BaseClient(server, username, password)
{
    public readonly HarborUserClient User = new(server, username, password);
    public readonly HarborProjectClient Project = new(server, username, password);
}
