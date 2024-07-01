using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clouded.Results;

public class SuccessCloudedResponse : ProducesResponseTypeAttribute
{
    public SuccessCloudedResponse()
        : base(typeof(CloudedOkResult), StatusCodes.Status200OK) { }

    public SuccessCloudedResponse(int statusCode)
        : base(typeof(CloudedOkResult), statusCode) { }
}

public class SuccessCloudedResponse<T> : ProducesResponseTypeAttribute
{
    public SuccessCloudedResponse()
        : base(typeof(CloudedOkResult<T>), StatusCodes.Status200OK) { }

    public SuccessCloudedResponse(int statusCode)
        : base(typeof(CloudedOkResult<T>), statusCode) { }
}
