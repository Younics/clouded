using Clouded.Auth.Provider.DataSources.Interfaces;

namespace Clouded.Auth.Provider;

public static class ApplicationExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder builder)
    {
        var services = builder.ApplicationServices.CreateScope().ServiceProvider;
        var userDataSourceService = services.GetRequiredService<IUserDataSource>();
        var domainDataSourceService = services.GetRequiredService<IDomainDataSource>();
        var machineDataSourceService = services.GetRequiredService<IMachineDataSource>();
        var organizationDataSourceService = services.GetRequiredService<IOrganizationDataSource>();
        var roleDataSourceService = services.GetRequiredService<IRoleDataSource>();
        var permissionDataSourceService = services.GetRequiredService<IPermissionDataSource>();

        userDataSourceService.TableVerification();
        userDataSourceService.SupportTableSetup();

        domainDataSourceService.TableVerification();
        domainDataSourceService.SupportTableSetup();

        machineDataSourceService.TableVerification();
        machineDataSourceService.SupportTableSetup();

        organizationDataSourceService.TableVerification();
        organizationDataSourceService.SupportTableSetup();

        roleDataSourceService.TableVerification();
        roleDataSourceService.SupportTableSetup();

        permissionDataSourceService.TableVerification();
    }
}
