using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Results.Exceptions;

namespace Clouded.Auth.Provider.Services.Base;

public abstract class BaseService
{
    /// <summary>
    /// Check if entity has been found
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityName"></param>
    /// <exception cref="NotFoundException"></exception>
    protected virtual void CheckEntityFound(BaseDictionary entity, string entityName)
    {
        if (!entity.Any())
            throw new NotFoundException(entityName);
    }
}
