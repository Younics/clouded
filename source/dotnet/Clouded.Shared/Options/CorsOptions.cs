namespace Clouded.Shared.Options;

public class CorsOptions
{
    public string[] AllowedMethods { get; set; }
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedHeaders { get; set; }
    public string[] ExposedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// A positive <see cref="int"/> indicating the seconds a preflight
    /// request can be cached.
    /// </summary>
    public int MaxAge { get; set; }

    public bool SupportsCredentials { get; set; }
}
