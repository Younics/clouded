using System.ComponentModel.DataAnnotations;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class ConditionOrInput : ICondition
{
    [Required]
    public IEnumerable<ICondition> Conditions { get; set; } = Array.Empty<ICondition>();
}
