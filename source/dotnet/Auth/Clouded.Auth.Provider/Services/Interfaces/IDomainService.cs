using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Domain;
using Clouded.Auth.Shared.Pagination;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IDomainService
{
    public Paginated<DomainDictionary> GetAllPaginated(object? search, int page = 0, int size = 50);

    public IEnumerable<DomainDictionary> GetAll(object? search);

    public DomainDictionary GetById(object id, bool withMachines);

    public DomainDictionary GetByIdentity(object identity, bool withMachines);

    public DomainDictionary Create(DomainDictionary data);

    public DomainDictionary Update(object id, DomainDictionary data);

    public void DeleteById(object id);

    public void AddMachines(object id, DomainMachineInput input);

    public void RemoveMachines(object id, DomainMachineInput input);
}
