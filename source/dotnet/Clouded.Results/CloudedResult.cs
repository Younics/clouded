using Clouded.Results.Exceptions;

namespace Clouded.Results;

public interface ICloudedResult { }

public class CloudedOkResult : ICloudedResult
{
    public bool Data = true;
}

public class CloudedOkResult<T> : CloudedOkResult
{
    /// <summary>
    /// Serialized result object
    /// </summary>
    public T? Data { get; set; }
}

public class CloudedExceptionResult : ICloudedResult
{
    /// <summary>
    /// Serialized array of exceptions
    /// </summary>
    public ExceptionData[]? Errors { get; set; }
}

public static class CloudedResultExtensions
{
    public static CloudedException ToCloudedException(
        this CloudedExceptionResult cloudedExceptionResult,
        int responseStatusCode
    )
    {
        var firstError = cloudedExceptionResult.Errors![0];
        return new CloudedException(firstError.Error, firstError.I18N, responseStatusCode);
    }
}
