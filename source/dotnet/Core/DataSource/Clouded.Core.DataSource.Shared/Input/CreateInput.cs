using System.ComponentModel.DataAnnotations;

namespace Clouded.Core.DataSource.Shared.Input;

public class CreateInput
{
    [Required]
    public string Schema { get; set; }

    [Required]
    public string Table { get; set; }

    [Required]
    public DataSourceDictionary Data { get; set; }

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
}
