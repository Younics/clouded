using Clouded.Platform.Models.Enums;

namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderHashInput
{
    public EHashType? AlgorithmType { get; set; }
    public string? Secret { get; set; }
}
