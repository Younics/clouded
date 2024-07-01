namespace Clouded.Admin.Provider.Contracts;

public class SettingsContract
{
    public RelationsConfigContract Relations { get; set; } = new();
    public ColumnViewsContract Views { get; set; } = new();
}
