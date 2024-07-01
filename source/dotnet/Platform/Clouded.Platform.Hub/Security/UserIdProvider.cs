using Microsoft.AspNetCore.SignalR;

namespace Clouded.Platform.Hub.Security;

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.Claims.FirstOrDefault(i => i.Type == "user_id")?.Value;
    }
}
