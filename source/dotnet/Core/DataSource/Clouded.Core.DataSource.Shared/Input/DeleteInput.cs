using System.ComponentModel.DataAnnotations;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class DeleteInput
{
    [Required]
    public string Schema { get; set; }

    [Required]
    public string Table { get; set; }

    [Required]
    public string Alias { get; set; }

    [Required]
    public ICondition? Where { get; set; }
}
