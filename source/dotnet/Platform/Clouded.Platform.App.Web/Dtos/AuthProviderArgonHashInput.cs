namespace Clouded.Platform.App.Web.Dtos;

public class AuthProviderArgonHashInput
{
    public int DegreeOfParallelism { get; set; } = 8;
    public int MemorySize { get; set; } = 32768;
    public int Iterations { get; set; } = 4;
    public int ReturnBytes { get; set; } = 128;
}
