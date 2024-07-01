using Clouded.Core.Tracking.Options;
using Clouded.Core.Tracking.Services;
using Clouded.Core.Tracking.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clouded.Core.Tracking;

public static class ServiceExtensions
{
    public static IServiceCollection AddTrackingService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        var trackingSection = configuration.GetSection("Clouded").GetSection("Tracking");
        services.Configure<TrackingOptions>(trackingSection.Bind);
        services.AddSingleton<ITrackingService, TrackingService>();

        return services;
    }
}
