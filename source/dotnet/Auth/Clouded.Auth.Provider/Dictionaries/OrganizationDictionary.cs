using Clouded.Auth.Provider.Dictionaries.Base;

namespace Clouded.Auth.Provider.Dictionaries;

public class OrganizationDictionary : BaseDictionary
{
    public IEnumerable<object?> Identities
    {
        get => Options!.ColumnIdentityArray.Select(x => this.GetValueOrDefault(x));
    }

    public OrganizationDictionary() => InitializeOptions(options => options.Organization);
}
