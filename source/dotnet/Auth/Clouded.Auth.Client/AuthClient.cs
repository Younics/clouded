using Clouded.Auth.Client.Base;

namespace Clouded.Auth.Client;

public class AuthClient(
    string apiUrl,
    string? apiKey = null,
    string? secretKey = null,
    bool untrustedSsl = false
) : BaseAuthClient(apiUrl, apiKey, secretKey, untrustedSsl: untrustedSsl);
