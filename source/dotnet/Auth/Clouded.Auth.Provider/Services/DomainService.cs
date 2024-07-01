using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Domain;
using Clouded.Auth.Shared.Pagination;
using Clouded.Function.Library;
using Clouded.Results.Exceptions;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class DomainService(
    ApplicationOptions options,
    IDomainDataSource dataSource,
    IMachineDataSource machineDataSource,
    HookResolver hookResolver
)
    : EntityBaseService<IDomainDataSource, DomainDictionary>("Domain", dataSource, hookResolver),
        IDomainService
{
    private readonly EntityIdentityDomainOptions _entityIdentityDomainOptions = options
        .Clouded
        .Auth
        .Identity
        .Domain!;

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Paginated<DomainDictionary> GetAllPaginated(object? search, int page = 0, int size = 50)
    {
        var domains = DataSource.EntitiesPaginated(page, size, search);

        return domains;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public IEnumerable<DomainDictionary> GetAll(object? search)
    {
        var domains = DataSource.Entities(search);

        return domains;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withMachines"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public DomainDictionary GetById(object id, bool withMachines)
    {
        var domain = DataSource.EntityFindById(id);
        CheckEntityFound(domain);

        if (withMachines)
            DataSource.DomainExtendWithMachines(domain);

        return domain;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withMachines"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns></returns>
    public DomainDictionary GetByIdentity(object identity, bool withMachines)
    {
        var domain = DataSource.EntityFindByIdentity(identity);
        CheckEntityFound(domain);

        if (withMachines)
            DataSource.DomainExtendWithMachines(domain);

        return domain;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    public DomainDictionary Create(DomainDictionary data)
    {
        data = data;

        // Domain validation
        CreateRecordValidation(_entityIdentityDomainOptions.ColumnIdentityArray, data);

        var domain = DataSource.EntityCreate(data);
        return domain;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public DomainDictionary Update(object id, DomainDictionary data)
    {
        id = id!;
        data = data;

        var domainToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(domainToBeUpdated);

        // Domain validation
        UpdateRecordValidation(
            _entityIdentityDomainOptions.ColumnIdentityArray,
            domainToBeUpdated,
            data
        );

        var domain = DataSource.EntityUpdate(id, data);
        return domain;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void AddMachines(object id, DomainMachineInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.MachineIds)
        {
            var relatedToBeUpdated = machineDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.DomainMachinesAdd(id, input.MachineIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public void RemoveMachines(object id, DomainMachineInput input)
    {
        var entityToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(entityToBeUpdated);

        foreach (var relatedEntityId in input.MachineIds)
        {
            var relatedToBeUpdated = machineDataSource.EntityFindById(relatedEntityId);
            CheckEntityFound(relatedToBeUpdated);
        }

        DataSource.DomainMachinesRemove(id, input.MachineIds);
    }
}
