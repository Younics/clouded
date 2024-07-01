using System.ComponentModel.DataAnnotations;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class ConditionValueInput : ICondition
{
    public string Id => $"{$"{Alias}_{Column}_{Mode}_{Value}".GetHashCode():X}";

    [Required]
    public required string Alias { get; set; }

    [Required]
    public required string Column { get; set; }

    [Required]
    public object? Value { get; set; }

    [Required]
    public EOperator Operator { get; set; }

    [Required]
    public EMode Mode { get; set; }

    public static ICondition GetDefaultCondition()
    {
        return new DefaultConditionInput();
    }
}
