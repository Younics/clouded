namespace Clouded.Results.Exceptions;

public class CloudedException : Exception
{
    public static string GeneralI18NMessage = "errors.clouded.general.message";
    public static string GeneralMessage = "Something went wrong. We are working to fix it.";
    public readonly Exception? OriginalException;
    public readonly int StatusCode;
    public readonly string I18N;

    public CloudedException(int statusCode)
        : base(GeneralMessage)
    {
        StatusCode = statusCode;
        I18N = GeneralI18NMessage;
    }

    public CloudedException(string message, string i18N, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
        I18N = $"errors.clouded.{i18N}";
    }

    public CloudedException(
        string message,
        string i18N,
        int statusCode,
        Exception originalException
    )
        : base(message)
    {
        StatusCode = statusCode;
        I18N = $"errors.clouded.{i18N}";
        OriginalException = originalException;
    }
}
