using System.Net;

namespace Clouded.Results.Exceptions;

public class NotSupportedException : CloudedException
{
    public NotSupportedException()
        : base("Not supported", "general.not_supported", (int)HttpStatusCode.NotImplemented) { }

    public NotSupportedException(string message)
        : base(message, "general.not_supported", (int)HttpStatusCode.NotImplemented) { }
}
