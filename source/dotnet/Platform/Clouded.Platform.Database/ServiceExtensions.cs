using Clouded.Platform.Database.Contexts.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Clouded.Platform.Database;

public static class ServiceExtensions
{
    public static IServiceCollection AddDbContextExtensions(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        services.AddScoped<IDbContextResolver, DbContextResolver>();

        return services;
    }
}
