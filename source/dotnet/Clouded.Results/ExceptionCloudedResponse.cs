using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Results;

public class ExceptionCloudedResponse : ProducesResponseTypeAttribute
{
    public ExceptionCloudedResponse()
        : base(typeof(CloudedExceptionResult), StatusCodes.Status400BadRequest) { }

    public ExceptionCloudedResponse(int statusCode)
        : base(typeof(CloudedExceptionResult), statusCode) { }
}
