using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Services.Base;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Auth.Shared.Organization;
using Clouded.Auth.Shared.Pagination;
using Clouded.Function.Library;
using Clouded.Results.Exceptions;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Services;

public class OrganizationService(
    ApplicationOptions options,
    IOrganizationDataSource dataSource,
    IUserDataSource userDataSource,
    HookResolver hookResolver
)
    : EntityBaseService<IOrganizationDataSource, OrganizationDictionary>(
        "Organization",
        dataSource,
        hookResolver
    ),
        IOrganizationService
{
    private readonly EntityIdentityOrganizationOptions _entityIdentityOrganizationOptions = options
        .Clouded
        .Auth
        .Identity
        .Organization!;

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Paginated<OrganizationDictionary> GetAllPaginated(
        object? search,
        int page = 0,
        int size = 50
    )
    {
        var organizations = DataSource.EntitiesPaginated(page, size, search);

        return organizations;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public IEnumerable<OrganizationDictionary> GetAll(object? search)
    {
        var organizations = DataSource.Entities(search);
        return organizations;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="withUsers"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    public OrganizationDictionary GetById(object id, bool withUsers = false)
    {
        var organization = DataSource.EntityFindById(id);
        CheckEntityFound(organization);

        if (withUsers)
            DataSource.OrganizationExtendWithUsers(organization);

        return organization;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="withUsers"></param>
    /// <exception cref="NotFoundException"></exception>
    /// <returns><see cref="OrganizationDictionary"/></returns>
    public OrganizationDictionary GetByIdentity(object identity, bool withUsers = false)
    {
        var organization = DataSource.EntityFindByIdentity(identity);
        CheckEntityFound(organization);

        if (withUsers)
            DataSource.OrganizationExtendWithUsers(organization);

        return organization;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    public OrganizationDictionary Create(OrganizationDictionary data)
    {
        data = data;

        // Organization validation
        CreateRecordValidation(_entityIdentityOrganizationOptions.ColumnIdentityArray, data);

        var organization = DataSource.EntityCreate(data);
        return organization;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ConflictException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public OrganizationDictionary Update(object id, OrganizationDictionary data)
    {
        id = id!;
        data = data;

        var organizationToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(organizationToBeUpdated);

        // Organization validation
        UpdateRecordValidation(
            _entityIdentityOrganizationOptions.ColumnIdentityArray,
            organizationToBeUpdated,
            data
        );

        var organization = DataSource.EntityUpdate(id, data);
        return organization;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public void AddUsers(object id, OrganizationUserInput input)
    {
        var organizationToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(organizationToBeUpdated);

        foreach (var inputUserId in input.UserIds)
        {
            var userToBeUpdated = userDataSource.EntityFindById(inputUserId);
            CheckEntityFound(userToBeUpdated);
        }

        DataSource.OrganizationUsersAdd(id, input.UserIds);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public void RemoveUsers(object id, OrganizationUserInput input)
    {
        var organizationToBeUpdated = DataSource.EntityFindById(id);
        CheckEntityFound(organizationToBeUpdated);

        foreach (var inputUserId in input.UserIds)
        {
            var userToBeUpdated = userDataSource.EntityFindById(inputUserId);
            CheckEntityFound(userToBeUpdated);
        }

        DataSource.OrganizationUsersRemove(id, input.UserIds);
    }
}
