using Clouded.Auth.Provider.Dictionaries;

namespace Clouded.Auth.Provider.DataSources.Interfaces;

public interface IOrganizationDataSource : IDataSource<OrganizationDictionary>
{
    public IEnumerable<UserDictionary> OrganizationUsers(object? organizationId);
    public void OrganizationExtendWithUsers(OrganizationDictionary organization);
    public void OrganizationUsersAdd(object? id, IEnumerable<object> userIds);
    public void OrganizationUsersRemove(object? id, IEnumerable<object> userIds);
}
