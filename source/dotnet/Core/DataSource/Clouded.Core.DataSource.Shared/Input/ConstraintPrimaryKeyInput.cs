using System.ComponentModel.DataAnnotations;

namespace Clouded.Core.DataSource.Shared.Input;

public class ConstraintPrimaryKeyInput
{
    [Required]
    public string Name { get; set; }

    [Required]
    public IEnumerable<string> Columns { get; set; } = Array.Empty<string>();
}
