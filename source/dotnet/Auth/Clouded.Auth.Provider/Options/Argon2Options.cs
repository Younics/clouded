using System.Text;

namespace Clouded.Auth.Provider.Options;

public class Argon2Options
{
    public string Secret { get; set; }
    public byte[] SecretBytes => Encoding.UTF8.GetBytes(Secret);

    public int DegreeOfParallelism { get; set; }
    public int MemorySize { get; set; }
    public int Iterations { get; set; }
    public int ReturnBytes { get; set; }
}
