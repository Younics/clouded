using System.Net;
using Clouded.Results.Exceptions;

namespace Clouded.Auth.Provider.Exceptions;

public class UnauthorizedException : CloudedException
{
    public UnauthorizedException()
        : base("Unauthorized", "auth.unauthorized", (int)HttpStatusCode.Unauthorized) { }

    public UnauthorizedException(string message, string i18N)
        : base(message, i18N, (int)HttpStatusCode.Unauthorized) { }
}
