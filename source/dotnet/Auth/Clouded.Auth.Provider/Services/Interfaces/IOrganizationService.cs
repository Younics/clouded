using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Shared.Organization;
using Clouded.Auth.Shared.Pagination;

namespace Clouded.Auth.Provider.Services.Interfaces;

public interface IOrganizationService
{
    public Paginated<OrganizationDictionary> GetAllPaginated(
        object? search,
        int page = 0,
        int size = 50
    );

    public IEnumerable<OrganizationDictionary> GetAll(object? search);

    public OrganizationDictionary GetById(object id, bool withUsers = false);
    public OrganizationDictionary GetByIdentity(object identity, bool withUsers = false);

    public OrganizationDictionary Create(OrganizationDictionary data);
    public OrganizationDictionary Update(object id, OrganizationDictionary data);
    public void DeleteById(object id);

    public void AddUsers(object id, OrganizationUserInput input);

    public void RemoveUsers(object id, OrganizationUserInput input);
}
