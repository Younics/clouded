using System.Net;

namespace Clouded.Results.Exceptions;

public class BadRequestException()
    : CloudedException($"Bad request!", "general.bad_request", (int)HttpStatusCode.BadRequest);
