using Clouded.Auth.Provider.DataSources.Base;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Dictionaries;
using Clouded.Auth.Provider.Options;
using Clouded.Auth.Provider.Options.Base;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

namespace Clouded.Auth.Provider.DataSources;

public class PermissionDataSource(ApplicationOptions options, IHashService hashService) :
    BaseDataSource<PermissionDictionary, EntityIdentityPermissionOptions, EntityMetaOptions, EntitySupportOptions>(
        options, hashService,
        options.Clouded.Auth.Identity.Permission == null
            ? []
            : [
                options.Clouded.Auth.Identity.Permission.ColumnId,
                options.Clouded.Auth.Identity.Permission.ColumnIdentity
            ],
        options.Clouded.Auth.Identity.Permission, options.Clouded.Auth.Identity.Permission?.Meta,
        options.Clouded.Auth.Identity.Permission?.Support), IPermissionDataSource
{
    public override void SupportTableSetup() { }
}
