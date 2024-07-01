using Clouded.Admin.Provider.DataSources.Interfaces;

namespace Clouded.Admin.Provider;

public static class ApplicationExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder builder)
    {
        var services = builder.ApplicationServices.CreateScope().ServiceProvider;
        var userSettingsDataSource = services.GetRequiredService<IUserSettingsDataSource>();

        userSettingsDataSource.TableVerification();
        userSettingsDataSource.SupportTableSetup();
    }
}
