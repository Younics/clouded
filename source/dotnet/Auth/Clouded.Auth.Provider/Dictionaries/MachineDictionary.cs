using Clouded.Auth.Provider.Dictionaries.Base;
using Clouded.Auth.Provider.Options;

namespace Clouded.Auth.Provider.Dictionaries;

public class MachineDictionary : BaseDictionary
{
    public MachineDictionary() => InitializeOptions(options => options.Machine);

    public string Type
    {
        get =>
            (
                this.GetValueOrDefault(((Options as EntityIdentityMachineOptions)!).ColumnType)!
                as string
            )!;
        set => this[((Options as EntityIdentityMachineOptions)!).ColumnType] = value;
    }
}
