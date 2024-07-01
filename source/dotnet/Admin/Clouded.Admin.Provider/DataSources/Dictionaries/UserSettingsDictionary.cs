using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;

namespace Clouded.Admin.Provider.DataSources.Dictionaries;

public class UserSettingsDictionary : DataSourceDictionary
{
    private readonly UserSettingsOptions _options;

    public object Id
    {
        get => this.GetValueOrDefault(_options.ColumnId)!;
        set => this[_options.ColumnId] = value;
    }

    public string? Settings
    {
        get => this.GetValueOrDefault((_options).ColumnSettings)! as string;
        set => this[(_options).ColumnSettings] = value;
    }

    public UserSettingsDictionary()
    {
        var scope = AppContext.CurrentHttpContext.RequestServices.CreateScope();
        var appOptions = scope.ServiceProvider.GetRequiredService<ApplicationOptions>();

        this._options = appOptions.Clouded.Admin.Auth.UserSettings;
        if (this._options == null)
        {
            throw new NotSupportedException();
        }
    }
}
