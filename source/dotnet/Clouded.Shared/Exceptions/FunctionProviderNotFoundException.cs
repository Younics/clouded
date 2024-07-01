namespace Clouded.Shared.Exceptions;

public class FunctionProviderNotFoundException(string functionName)
    : Exception($"Function provider for function: {functionName} not found.");
