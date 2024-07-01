using Clouded.Auth.Client.Base;

namespace Clouded.Auth.Client;

internal sealed class AuthCloudedClient(
    string apiUrl,
    string? cloudedKey = null,
    bool untrustedSsl = false
) : BaseAuthClient(apiUrl, cloudedKey: cloudedKey, untrustedSsl: untrustedSsl);
