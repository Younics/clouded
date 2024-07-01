using System.Net;

namespace Clouded.Results.Exceptions;

public class NotFoundException(string entityName)
    : CloudedException($"{entityName} not found", "entity.not_found", (int)HttpStatusCode.NotFound);
