using System.Globalization;
using System.Text;
using Clouded.Shared.Cryptography;
using Microsoft.Net.Http.Headers;

namespace WebSupport.Library;

public abstract class BaseClient(string apiKey, string secret)
{
    protected const string ApiUri = "https://rest.websupport.sk";
    protected string ApiKey { get; } = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    protected string Secret { get; } = secret ?? throw new ArgumentNullException(nameof(secret));

    private readonly ICrypto _hmacSha1 = new CryptoHmacSha1();

    protected IDictionary<string, string> MakeHeaders(HttpMethod method, string path)
    {
        var dateTime = DateTimeOffset.Now;
        var canonicalRequest = $"{method.ToString()} {path} {dateTime.ToUnixTimeSeconds()}";
        var signature = _hmacSha1.Hash(canonicalRequest, Secret);
        var token = Encoding.ASCII.GetBytes($"{ApiKey}:{signature}");

        return new Dictionary<string, string>
        {
            [HeaderNames.ContentType] = "application/json",
            [HeaderNames.Accept] = "application/json",
            [HeaderNames.Authorization] = $"Basic {Convert.ToBase64String(token)}",
            [HeaderNames.Date] = dateTime.ToString("o", CultureInfo.InvariantCulture)
        };
    }
}
