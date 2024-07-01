using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;

namespace Clouded.Auth.Provider.Dictionaries;

public class PermissionDictionary : BaseDictionary
{
    public PermissionDictionary() => InitializeOptions(options => options.Permission);

    public string Name
    {
        get =>
            (
                this.GetValueOrDefault(
                    ((Options as EntityIdentityPermissionOptions)!).ColumnIdentity
                )! as string
            )!;
        set => this[((Options as EntityIdentityPermissionOptions)!).ColumnIdentity] = value;
    }
}
