using System.Text.Json;
using Clouded.Admin.Provider.Contexts;
using Clouded.Admin.Provider.Contracts;
using Clouded.Admin.Provider.DataSources.Dictionaries;
using Clouded.Admin.Provider.DataSources.Interfaces;
using Clouded.Admin.Provider.Options;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Enums;
using Clouded.Core.DataSource.Shared.Input;

namespace Clouded.Admin.Provider.Helpers;

public static class RelationHelper
{
    public static async Task<Dictionary<RelationResult, IEnumerable<string>>> LoadRelationsConfigs(
        this IEnumerable<RelationResult> relations,
        string userId,
        IUserSettingsDataSource userSettingsDataSource
    )
    {
        var relationsConfigs = new Dictionary<RelationResult, IEnumerable<string>>();

        foreach (var relationResult in relations)
        {
            relationsConfigs.Add(
                relationResult,
                await LoadRelationConfig(relationResult, userId, userSettingsDataSource)
            );
        }

        return relationsConfigs;
    }

    public static IEnumerable<RelationResult> OnlyEnabled(
        this IEnumerable<RelationResult> relations,
        List<TableOptions> tables
    )
    {
        return relations.Where(
            x => tables.Exists(y => y.Schema == x.TargetSchema && y.Table == x.TargetTable)
        );
    }

    public static void CascadeDeleteRelations(
        this List<RelationResult> outsideRelations,
        DataSourceDictionary parentEntity,
        AdminContext context
    )
    {
        foreach (
            var outsideRelation in outsideRelations!.Where(
                outsideRelation => outsideRelation.TargetColumnNotNull
            )
        )
        {
            var nestedOutsideRelations = context!
                .GetOutsideRelations(outsideRelation.TargetSchema, outsideRelation.TargetTable)
                .ToList();
            var outsideRelationEntities = context!
                .Select(
                    new SelectInput
                    {
                        Schema = outsideRelation.TargetSchema,
                        Table = outsideRelation.TargetTable,
                        Alias = outsideRelation.TargetTable,
                        Where = new ConditionValueInput
                        {
                            Alias = outsideRelation.TargetTable,
                            Column = outsideRelation.TargetColumn,
                            Operator = EOperator.Equals,
                            Value = parentEntity[outsideRelation.Column]
                        }
                    }
                )
                .ToList();
            foreach (var outsideRelationEntity in outsideRelationEntities)
            {
                CascadeDeleteRelations(nestedOutsideRelations, outsideRelationEntity, context);

                context!.Delete(
                    new DeleteInput
                    {
                        Schema = outsideRelation.TargetSchema,
                        Table = outsideRelation.TargetTable,
                        Alias = outsideRelation.TargetTable,
                        Where = new ConditionValueInput
                        {
                            Alias = outsideRelation.TargetTable,
                            Column = outsideRelation.TargetColumn,
                            Operator = EOperator.Equals,
                            Value = outsideRelationEntity[outsideRelation.TargetColumn]
                        }
                    }
                );
            }
        }
    }

    public static async Task<IEnumerable<string>> LoadRelationConfig(
        this RelationResult relation,
        string userId,
        IUserSettingsDataSource userSettingsDataSource,
        bool attachDialog = false
    )
    {
        var settingsConfig = new SettingsContract();

        var storedString = userSettingsDataSource.GetByUserId(userId)?.Settings;
        if (!string.IsNullOrEmpty(storedString))
        {
            settingsConfig = JsonSerializer.Deserialize<SettingsContract>(storedString);
        }

        var relationConfig = settingsConfig!.Relations!.GetValueOrDefault(
            GetRelationConfigKey(relation!, attachDialog),
            null
        );

        if (relationConfig != null)
        {
            return relationConfig;
        }

        return new[] { relation.TargetColumn };
    }

    public static void SetRelationConfig(
        this RelationResult relation,
        UserSettingsOptions options,
        string userId,
        IUserSettingsDataSource userSettingsDataSource,
        IEnumerable<string> columnNames,
        bool attachDialog = false
    )
    {
        var settingsConfig = new SettingsContract();
        var relationsConfig = new RelationsConfigContract();

        var storedString = userSettingsDataSource.GetByUserId(userId)?.Settings;
        if (!string.IsNullOrEmpty(storedString))
        {
            settingsConfig = JsonSerializer.Deserialize<SettingsContract>(storedString);

            if (settingsConfig != null)
            {
                relationsConfig = settingsConfig.Relations;
            }
        }

        relationsConfig[relation.GetRelationConfigKey(attachDialog)] = columnNames;
        settingsConfig!.Relations = relationsConfig;

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

    private static string GetRelationConfigKey(
        this RelationResult relation,
        bool attachDialog = false
    )
    {
        if (attachDialog)
        {
            return $"{relation.Schema}-{relation.Table}-{relation.Column}-{relation.TargetSchema}-{relation.TargetTable}-{relation.TargetColumn}-attach";
        }

        return $"{relation.Schema}-{relation.Table}-{relation.Column}-{relation.TargetSchema}-{relation.TargetTable}-{relation.TargetColumn}";
    }
}
