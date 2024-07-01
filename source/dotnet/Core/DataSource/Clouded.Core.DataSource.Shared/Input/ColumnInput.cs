using Clouded.Shared.Enums;

namespace Clouded.Core.DataSource.Shared.Input;

public class ColumnInput
{
    public required string Name { get; set; }

    public EColumnType? Type { get; set; }

    public string? TypeRaw { get; set; }

    public bool NotNull { get; set; }

    public ConstraintReferenceInput? Reference { get; set; }

    public ConstraintPrimaryKeyInput? PrimaryKey { get; set; }

    public ConstraintUniqueKeyInput? UniqueKey { get; set; }
}
