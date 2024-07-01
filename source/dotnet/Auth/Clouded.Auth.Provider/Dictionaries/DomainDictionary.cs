using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;

namespace Clouded.Auth.Provider.Dictionaries;

public class DomainDictionary : BaseDictionary
{
    public DomainDictionary() => InitializeOptions(options => options.Domain);

    public object Value
    {
        get => this.GetValueOrDefault(((Options as EntityIdentityDomainOptions)!).ColumnIdentity)!;
        set => this[((Options as EntityIdentityDomainOptions)!).ColumnIdentity] = value;
    }
}
