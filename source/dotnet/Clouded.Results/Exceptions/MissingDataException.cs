using System.Net;

namespace Clouded.Results.Exceptions;

public class MissingDataException(string attribute)
    : CloudedException(
        $"Entity miss {attribute}",
        $"general.miss_data.{attribute}",
        (int)HttpStatusCode.UnprocessableEntity
    );
