// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Clouded.Core.License;

var appType = "Auth";
var expiresIn = DateTime.UtcNow.AddYears(10);

// var key = LicenseTool.GenerateRsaKey();
var privateKey =
    "BwIAAACkAABSU0EyAAQAAAEAAQCZ8S4XyM2oj4PVmTTuBig/rxymPy1PyA+BhsfcwRxfDA/BWTKL80VUvJrmVGDDzZRkB84Jd0kyiZPPzvuCyYGbtGgUtcFWZ+uT7OaiGHo6Hv//a7VS9R64Yw2JWlfCxKBlXYBDoTaqbYWUL63hqEb2d739f/iBqtRcRd+GjpyMpX/gcmuMBxR8VGY22R+91baRdaGVUP1i/SeB7ItpHC/tFvyPQzD1LONowlnZFn1/ectagKkhxXiY2D131W1NXebnIWohSIx2O21bnmdSjSHqzWny03b/Pl2pmGjFF/Aph4ARSS296bXDOqI2KNoccNjMeJcdkatUfPCNoaQq1fi3Fft9LnQTtZSA+ylVvkKMUJ1OxgfIJQRUGKDPeI6cJwOZAUpaRRqfpK3fHdsoHX8Tvs+XnEflpD70aR4nRo3CHHEHQL2T22DNRNy077Iq1nyCNiF1f9sJq5k2/gN8B+BNmFzvvbEq3Iank6aFNzVfi6wimtx4JAVMyrrPMODnsRPYNsiw2qVnp6axYbbXGIH+C/IraEVH5UpYBhpwU4rlVKzdH6c2Fd8QFgDQHpAx84XBax9t7ueAIjl2RMw+o+2UMSEelzF1M7RrxKh5YP2wodmYA5ffrHI2hdjRFeIUYw0DsAyZPFRyyLpxd/5KNbdntXDL0b1G41+5EcCwemVb0BuVXc1QHNtjGlYogyOmM8rZInaZewOjtcT0tiMGKTnaMLmlgAAw7eSy/YfM/IMXkjcLO8Sikp1sy8kK2w6r5A0=";
var publicKey =
    "BgIAAACkAABSU0ExAAQAAAEAAQCZ8S4XyM2oj4PVmTTuBig/rxymPy1PyA+BhsfcwRxfDA/BWTKL80VUvJrmVGDDzZRkB84Jd0kyiZPPzvuCyYGbtGgUtcFWZ+uT7OaiGHo6Hv//a7VS9R64Yw2JWlfCxKBlXYBDoTaqbYWUL63hqEb2d739f/iBqtRcRd+GjpyMpQ==";
var license = LicenseTool.GenerateLicense(appType, privateKey, expiresIn);

try
{
    LicenseTool.VerifyLicense(appType, publicKey, license);
}
catch (Exception e)
{
    Debug.WriteLine(e);
}
