using System.Text.Json;
using Clouded.Admin.Provider.Contracts;
using Clouded.Admin.Provider.DataSources.Dictionaries;
using Clouded.Admin.Provider.DataSources.Interfaces;
using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;

namespace Clouded.Admin.Provider.Helpers;

public static class ViewHelper
{
    public static async Task<Dictionary<string, bool>> LoadViewConfigs(
        this IEnumerable<TableColumnOptions> columns,
        TableOptions table,
        string userId,
        IUserSettingsDataSource userSettingsDataSource,
        RelationResult? relation
    )
    {
        var columnConfigs = new Dictionary<string, bool>();

        var settingsConfig = new SettingsContract();

        var storedString = userSettingsDataSource.GetByUserId(userId)?.Settings;
        if (!string.IsNullOrEmpty(storedString))
        {
            settingsConfig = JsonSerializer.Deserialize<SettingsContract>(storedString);
        }

        foreach (var tableColumnOptions in columns)
        {
            var tableViewConfig = settingsConfig!.Views?.GetValueOrDefault(
                table.GetViewsConfigKey(relation),
                new()
            );

            columnConfigs.Add(
                tableColumnOptions.Column,
                tableViewConfig!.GetValueOrDefault(tableColumnOptions.Column, true)
            );
        }

        return columnConfigs;
    }

    public static void SetViewsConfig(
        this TableOptions table,
        UserSettingsOptions options,
        string userId,
        IUserSettingsDataSource userSettingsDataSource,
        Dictionary<string, bool> viewConfigs,
        RelationResult? relation
    )
    {
        var settingsConfig = new SettingsContract();
        var viewConfig = new ColumnViewsContract();

        var storedString = userSettingsDataSource.GetByUserId(userId)?.Settings;
        if (!string.IsNullOrEmpty(storedString))
        {
            settingsConfig = JsonSerializer.Deserialize<SettingsContract>(storedString);

            if (settingsConfig != null)
            {
                viewConfig = settingsConfig.Views;
            }
        }

        viewConfig[table.GetViewsConfigKey(relation)] = viewConfigs;
        settingsConfig!.Views = viewConfig;

        try
        {
            if (!string.IsNullOrEmpty(storedString))
            {
                userSettingsDataSource.Update(
                    userId,
                    new UserSettingsDictionary
                    {
                        { options.ColumnId, userId },
                        { options.ColumnSettings, JsonSerializer.Serialize(settingsConfig) }
                    }
                );
            }
            else
            {
                userSettingsDataSource.Create(
                    new UserSettingsDictionary
                    {
                        { options.ColumnId, userId },
                        { options.ColumnSettings, JsonSerializer.Serialize(settingsConfig) }
                    }
                );
            }
        }
        catch (Exception e)
        {
            //potential read-only DB connection
            Console.WriteLine(e);
        }
    }

    private static string GetViewsConfigKey(this TableOptions table, RelationResult? relation)
    {
        if (relation != null)
        {
            return $"{table.Schema}-{table.Table}-{relation.Schema}-{relation.Table}";
        }

        return $"{table.Schema}-{table.Table}";
    }
}
