using System.Net;
using Clouded.Results.Exceptions;

namespace Clouded.Auth.Provider.Exceptions;

public class InvalidTokenException : CloudedException
{
    public InvalidTokenException()
        : base("Invalid token", "auth.invalid_token", (int)HttpStatusCode.BadRequest) { }

    public InvalidTokenException(Exception originalException)
        : base(
            "Invalid token",
            "auth.invalid_token",
            (int)HttpStatusCode.BadRequest,
            originalException
        ) { }
}
