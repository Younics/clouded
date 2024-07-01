using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Clouded.Shared;

public static class HttpContextExtensions
{
    public static string GetBearerToken(this HttpContext context, bool truncated = true)
    {
        var token = context.Request.Headers[HeaderNames.Authorization].ToString();
        return truncated ? Regex.Replace(token, @"^Bearer ", string.Empty) : token;
    }
}
