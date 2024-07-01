using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Function.Library;
using Clouded.Results.Exceptions;

namespace Clouded.Auth.Provider.Services.Base;

public class EntityBaseService<T, TDictionary> : BaseService
    where T : IDataSource<TDictionary>
    where TDictionary : BaseDictionary, new()
{
    protected readonly string EntityName;
    protected readonly T DataSource;
    protected readonly HookResolver HookResolver;

    protected EntityBaseService(string entityName, T dataSource, HookResolver hookResolver)
    {
        EntityName = entityName;
        DataSource = dataSource;
        HookResolver = hookResolver;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void DeleteById(object id)
    {
        DataSource.EntityDelete(id);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="columnIdentityArray"></param>
    /// <param name="newEntityData"></param>
    /// <exception cref="ConflictException"></exception>
    protected void CreateRecordValidation(
        IEnumerable<string> columnIdentityArray,
        TDictionary newEntityData
    )
    {
        var entityColumnsToVerify = columnIdentityArray
            .Where(column => newEntityData.GetValueOrDefault(column, null) != null)
            .Select(column => new KeyValuePair<string, object?>(column, newEntityData[column]))
            .ToDataSourceDictionary<TDictionary>();

        if (!entityColumnsToVerify.Any())
            return;

        var entityExists = DataSource.EntityExists(entityColumnsToVerify, true);
        if (entityExists)
            throw new ConflictException(EntityName);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="columnIdentityArray"></param>
    /// <param name="entityToBeUpdated"></param>
    /// <param name="newEntityData"></param>
    /// <exception cref="ConflictException"></exception>
    protected void UpdateRecordValidation(
        IEnumerable<string> columnIdentityArray,
        TDictionary entityToBeUpdated,
        TDictionary newEntityData
    )
    {
        var identityArray = columnIdentityArray.ToList();

        var possibleEntityConflict = identityArray
            .Where(column => newEntityData.GetValueOrDefault(column, null) != null)
            .Any(column =>
            {
                var value = newEntityData.GetValueOrDefault(column, null);
                value = value.Transform();

                var oldValue = entityToBeUpdated.GetValueOrDefault(column, null);
                return !Equals(value, oldValue);
            });

        if (!possibleEntityConflict)
            return;

        var entityColumnsToVerify = identityArray
            .Where(column => newEntityData.GetValueOrDefault(column, null) != null)
            .Select(column => new KeyValuePair<string, object?>(column, newEntityData[column]))
            .ToDataSourceDictionary<TDictionary>();

        if (!entityColumnsToVerify.Any())
            return;

        var entityExists = DataSource.EntityExists(entityColumnsToVerify, true);
        if (entityExists)
            throw new ConflictException(EntityName);
    }

    /// <summary>
    /// Check if entity has been found
    /// </summary>
    /// <param name="entity"></param>
    /// <exception cref="NotFoundException"></exception>
    protected void CheckEntityFound(BaseDictionary entity)
    {
        base.CheckEntityFound(entity, EntityName);
    }
}
