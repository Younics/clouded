using Clouded.Auth.Provider.Dictionaries.Base;

namespace Clouded.Auth.Provider.Dictionaries;

public class RoleDictionary : BaseDictionary
{
    public RoleDictionary() => InitializeOptions(options => options.Role);
}
