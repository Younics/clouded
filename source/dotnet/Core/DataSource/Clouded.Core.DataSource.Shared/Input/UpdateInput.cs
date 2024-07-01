using System.ComponentModel.DataAnnotations;
using Clouded.Core.DataSource.Shared.Interfaces;

namespace Clouded.Core.DataSource.Shared.Input;

public class UpdateInput
{
    [Required]
    public string Schema { get; set; } = null!;

    [Required]
    public string Table { get; set; } = null!;

    [Required]
    public string Alias { get; set; } = null!;

    [Required]
    public IEnumerable<string> ReturnColumns { get; set; } = Array.Empty<string>();

    public string? ReturnColumnsRaw
    {
        get
        {
            if (ReturnColumns.Any())
                return string.Join(
                    ',',
                    ReturnColumns.Select(column => column == "*" ? column : $""" "{column}" """)
                );

            return null;
        }
    }

    [Required]
    public DataSourceDictionary Data { get; set; }

    [Required]
    public ICondition? Where { get; set; }
}
