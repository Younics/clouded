namespace Clouded.Platform.Models;

public static class Env
{
    public static string? Profile => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public static bool IsDevelopment => Profile == "Development";

    public static bool IsProduction => Profile == "Production";
}
