using System.ComponentModel.DataAnnotations;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class ConditionColumnInput : ICondition
{
    [Required]
    public string TargetAlias { get; set; }

    [Required]
    public string TargetColumn { get; set; }

    [Required]
    public EOperator Operator { get; set; }

    [Required]
    public EMode Mode { get; set; }

    [Required]
    public string Alias { get; set; }

    [Required]
    public string Column { get; set; }
}
