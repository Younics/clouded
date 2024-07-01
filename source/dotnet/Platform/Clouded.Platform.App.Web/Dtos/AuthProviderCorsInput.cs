namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderCorsInput
{
    public long? Id { get; set; }
    public IEnumerable<string> AllowedMethods { get; set; } = Array.Empty<string>();
    public List<string> AllowedOrigins { get; set; } = new() { "*" };
    public List<string> AllowedHeaders { get; set; } = new();
    public List<string> ExposedHeaders { get; set; } = new();
    public bool SupportsCredentials { get; set; }
    public int MaxAge { get; set; }
}
