using System.Text;
using Flurl;
using Flurl.Http;
using Microsoft.Net.Http.Headers;

namespace Harbor.Library;

public abstract class BaseClient(string server, string username, string password)
{
    private string Server { get; } = server ?? throw new ArgumentNullException(nameof(server));
    private string Username { get; } =
        username ?? throw new ArgumentNullException(nameof(username));
    private string Password { get; } =
        password ?? throw new ArgumentNullException(nameof(password));

    private IDictionary<string, string> Headers
    {
        get
        {
            var token = Encoding.ASCII.GetBytes($"{Username}:{Password}");

            return new Dictionary<string, string>
            {
                [HeaderNames.ContentType] = "application/json",
                [HeaderNames.Accept] = "application/json",
                [HeaderNames.Authorization] = $"Basic {Convert.ToBase64String(token)}",
            };
        }
    }

    protected IFlurlRequest Request(params object[] pathSegments) =>
        Server.AppendPathSegment("/api").AppendPathSegments(pathSegments).WithHeaders(Headers);
}
