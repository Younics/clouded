using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Clouded.Platform.Database;

public static class ApplicationExtensions
{
    public static IServiceProvider Migrate(
        this IServiceProvider serviceProvider,
        params Type[] contextTypes
    )
    {
        if (serviceProvider is null)
            throw new ArgumentNullException(nameof(serviceProvider));

        var scope = serviceProvider.CreateScope();

        foreach (var contextType in contextTypes)
        {
            if (scope.ServiceProvider.GetRequiredService(contextType) is DbContext context)
                context.Database.Migrate();
            else
                throw new NotSupportedException("Context must be implemented from DbContext!");
        }

        return serviceProvider;
    }
}
