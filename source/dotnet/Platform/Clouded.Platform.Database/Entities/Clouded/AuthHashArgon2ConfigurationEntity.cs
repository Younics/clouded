using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clouded.Platform.Database.Entities.Clouded;

public class AuthHashArgon2ConfigurationEntity : AuthHashConfigurationEntity
{
    [Required]
    [Column("argon2_type")]
    public string Type { get; set; } = "DataIndependentAddressing";

    [Required]
    [Column("argon2_version")]
    public string Version { get; set; } = "Nineteen";

    [Required]
    [Column("argon2_degree_of_parallelism")]
    public int DegreeOfParallelism { get; set; } = 8;

    [Required]
    [Column("argon2_memory_size")]
    public int MemorySize { get; set; } = 32768;

    [Required]
    [Column("argon2_iterations")]
    public int Iterations { get; set; } = 4;

    [Required]
    [Column("argon2_return_bytes")]
    public int ReturnBytes { get; set; } = 128;
}
