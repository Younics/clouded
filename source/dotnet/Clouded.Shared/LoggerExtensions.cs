using Microsoft.Extensions.Logging;

namespace Clouded.Shared;

public static class LoggerExtensions
{
    // Extension method to create a LoggerProvider from
    // an existing ILogger
    public static ILoggerProvider AsLoggerProvider(this ILogger logger) =>
        new ExistingLoggerProvider(logger);

    private class ExistingLoggerProvider(ILogger logger) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return logger;
        }

        public void Dispose() { }
    }
}
