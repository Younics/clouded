using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Core.DataSource.Shared;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.Dictionaries.Base;

public class BaseDictionary : DataSourceDictionary
{
    public object Id
    {
        get => this.GetValueOrDefault(Options!.ColumnId)!;
        set => this[Options!.ColumnId] = value;
    }

    public object Identity
    {
        get => this.GetValueOrDefault(Options!.ColumnIdentity)!;
        set => this[Options!.ColumnIdentity] = value;
    }

    public string Schema
    {
        get => (string)this.GetValueOrDefault(Options!.Schema)!;
        set => this[Options!.Schema] = value;
    }

    public string Table
    {
        get => (string)this.GetValueOrDefault(Options!.Table)!;
        set => this[Options!.Table] = value;
    }

    protected BaseEntityIdentityOptions? Options { get; set; }

    protected void InitializeOptions(Func<IdentityOptions, BaseEntityIdentityOptions?> getOptions)
    {
        var scope = AppContext.CurrentHttpContext.RequestServices.CreateScope();
        var appOptions = scope.ServiceProvider.GetRequiredService<ApplicationOptions>();

        this.Options = getOptions(appOptions.Clouded.Auth.Identity);
        if (this.Options == null)
        {
            throw new NotSupportedException();
        }
    }
}
