using System.ComponentModel.DataAnnotations;

namespace Clouded.Core.DataSource.Shared.Input;

public class ConstraintReferenceInput
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string TargetSchema { get; set; }

    [Required]
    public string TargetTable { get; set; }

    [Required]
    public string TargetColumn { get; set; }

    public ActionType OnUpdate { get; set; }
    public ActionType OnDelete { get; set; }
}
