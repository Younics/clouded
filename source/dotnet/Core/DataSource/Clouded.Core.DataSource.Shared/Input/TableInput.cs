using System.ComponentModel.DataAnnotations;

namespace Clouded.Core.DataSource.Shared.Input;

public class TableInput
{
    [Required]
    public string Schema { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public IEnumerable<ColumnInput> Columns { get; set; } = Array.Empty<ColumnInput>();

    [Required]
    public IEnumerable<ConstraintReferenceInput> References { get; set; } =
        Array.Empty<ConstraintReferenceInput>();

    [Required]
    public IEnumerable<ConstraintPrimaryKeyInput> PrimaryKeys { get; set; } =
        Array.Empty<ConstraintPrimaryKeyInput>();

    [Required]
    public IEnumerable<ConstraintUniqueKeyInput> UniqueKeys { get; set; } =
        Array.Empty<ConstraintUniqueKeyInput>();
}
