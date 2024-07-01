namespace WebSupport.Library;

public class WebSupportClient(string apiKey, string secret) : BaseClient(apiKey, secret)
{
    public readonly WebSupportUserClient User = new(apiKey, secret);
    public readonly WebSupportDnsClient Dns = new(apiKey, secret);
}
