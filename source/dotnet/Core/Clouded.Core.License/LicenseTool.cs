using ThinkSharp.Licensing;
using ThinkSharp.Licensing.Signing;

namespace Clouded.Core.License;

public static class LicenseTool
{
    public static SigningKeyPair GenerateRsaKey() => Lic.KeyGenerator.GenerateRsaKeyPair();

    public static string GenerateLicense(
        string appType,
        string privateKey,
        DateTime? expirationDate = null
    )
    {
        var licenseBuilder = Lic.Builder
            .WithRsaPrivateKey(privateKey)
            .WithoutHardwareIdentifier()
            .WithoutSerialNumber();

        var afterExpirationBuilder = expirationDate.HasValue
            ? licenseBuilder.ExpiresOn(expirationDate.Value)
            : licenseBuilder.WithoutExpiration();

        return afterExpirationBuilder
            .WithProperty("Application", "Clouded")
            .WithProperty("Type", appType)
            .SignAndCreate()
            .Serialize();
    }

    public static void VerifyLicense(string appType, string publicKey, string licenseText)
    {
        var license = Lic.Verifier
            .WithRsaPublicKey(publicKey)
            .WithoutApplicationCode()
            .LoadAndVerify(licenseText);

        if (license.HasExpirationDate && license.ExpirationDate < DateTime.UtcNow)
            throw new SignedLicenseException("License is expired.");

        if (license.Properties["Application"] != "Clouded" || license.Properties["Type"] != appType)
            throw new SignedLicenseException("License is invalid.");
    }
}
