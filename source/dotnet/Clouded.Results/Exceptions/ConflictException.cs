using System.Net;

namespace Clouded.Results.Exceptions;

public class ConflictException(string entity)
    : CloudedException(
        $"{entity} already exists",
        "general.already_exists",
        (int)HttpStatusCode.Conflict
    );
