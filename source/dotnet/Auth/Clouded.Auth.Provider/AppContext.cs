namespace Clouded.Auth.Provider;

public static class AppContext
{
    private static IHttpContextAccessor _httpContextAccessor = null!;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static HttpContext CurrentHttpContext => _httpContextAccessor.HttpContext!;
}
