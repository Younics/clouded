using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Clouded.Shared;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TOptions : class, new()
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddOptions<TOptions>();

        return services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<TOptions>>().Value;
            configuration.Bind(options);
            return options;
        });
    }
}
