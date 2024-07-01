using Clouded.Admin.Provider.DataSources.Dictionaries;
using Clouded.Admin.Provider.DataSources.Interfaces;
using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Api;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;
using Clouded.Shared.Enums;

namespace Clouded.Admin.Provider.DataSources;

public class UserSettingsDataSource : IUserSettingsDataSource
{
    private readonly Context _context;
    private readonly ApplicationOptions _options;

    public UserSettingsDataSource(ApplicationOptions options)
    {
        var dataSources = options.Clouded.DataSources.ToList();
        var dataSource = dataSources.First(
            x => x.Id == options.Clouded.Admin.Auth.UserSettings.DataSourceId
        );
        _context = new Context(
            new Connection
            {
                Type = dataSource.Type,
                Server = dataSource.Server,
                Port = dataSource.Port,
                Username = dataSource.Username,
                Password = dataSource.Password,
                Database = dataSource.Database
            }
        );

        _options = options;
    }

    public virtual void Create(UserSettingsDictionary data)
    {
        _context.Create(
            new CreateInput
            {
                Schema = _options.Clouded.Admin.Auth.UserSettings.Schema,
                Table = _options.Clouded.Admin.Auth.UserSettings.Table,
                ReturnColumns = new[] { "*" },
                Data = data,
            }
        );
    }

    public virtual void Update(string userId, UserSettingsDictionary data)
    {
        _context.Update(
            new UpdateInput
            {
                Schema = _options.Clouded.Admin.Auth.UserSettings.Schema,
                Table = _options.Clouded.Admin.Auth.UserSettings.Table,
                Alias = _options.Clouded.Admin.Auth.UserSettings.Table,
                ReturnColumns = new[] { "*" },
                Data = data,
                Where = new ConditionValueInput
                {
                    Alias = _options.Clouded.Admin.Auth.UserSettings.Table,
                    Column = _options.Clouded.Admin.Auth.UserSettings.ColumnId,
                    Operator = EOperator.Equals,
                    Value = userId,
                }
            }
        );
    }

    public UserSettingsDictionary? GetByUserId(string userId)
    {
        return _context.SelectSingle<UserSettingsDictionary>(
            new SelectInput
            {
                Schema = _options.Clouded.Admin.Auth.UserSettings.Schema,
                Table = _options.Clouded.Admin.Auth.UserSettings.Table,
                Alias = _options.Clouded.Admin.Auth.UserSettings.Table,
                Where = new ConditionAndInput
                {
                    Conditions = new[]
                    {
                        new ConditionValueInput
                        {
                            Alias = _options.Clouded.Admin.Auth.UserSettings.Table,
                            Column = _options.Clouded.Admin.Auth.UserSettings.ColumnId,
                            Operator = EOperator.Equals,
                            Value = userId,
                        }
                    }
                }
            }
        );
    }

    public void TableVerification()
    {
        // Skip validation, because create is using IfNotExists flag
    }

    public void SupportTableSetup()
    {
        _context.CreateTable(
            new TableInput
            {
                Schema = _options.Clouded.Admin.Auth.UserSettings.Schema,
                Name = _options.Clouded.Admin.Auth.UserSettings.Table,
                Columns = new[]
                {
                    new ColumnInput
                    {
                        Name = _options.Clouded.Admin.Auth.UserSettings.ColumnId,
                        Type = EColumnType.Varchar,
                        NotNull = true
                    },
                    new ColumnInput
                    {
                        Name = _options.Clouded.Admin.Auth.UserSettings.ColumnSettings,
                        Type = EColumnType.Text
                    },
                },
                PrimaryKeys = new[]
                {
                    new ConstraintPrimaryKeyInput
                    {
                        Name = $"{_options.Clouded.Admin.Auth.UserSettings.Table}__id",
                        Columns = new[] { _options.Clouded.Admin.Auth.UserSettings.ColumnId }
                    }
                }
            },
            true
        );
    }
}
