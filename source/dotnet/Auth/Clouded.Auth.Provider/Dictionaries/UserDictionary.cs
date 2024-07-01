using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;

namespace Clouded.Auth.Provider.Dictionaries;

public class UserDictionary : BaseDictionary
{
    public IEnumerable<object?> Identities
    {
        get => Options!.ColumnIdentityArray.Select(x => this.GetValueOrDefault(x));
    }

    public object? Password
    {
        get => this.GetValueOrDefault(((Options as EntityIdentityUserOptions)!).ColumnPassword)!;
        set => this[((Options as EntityIdentityUserOptions)!).ColumnPassword] = value;
    }

    public UserDictionary() => InitializeOptions(options => options.User);
}
