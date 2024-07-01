using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Core.Tracking;
using Clouded.Platform.App.Web.Mappers;
using Clouded.Platform.App.Web.Options;
using Clouded.Platform.App.Web.Providers;
using Clouded.Platform.App.Web.Services.Interfaces;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MudBlazor;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.ConfigureOptions<ApplicationOptions>(builder.Configuration);
var applicationOptions = builder.Configuration.Get<ApplicationOptions>();
var dataSource = applicationOptions.Clouded.Database.CloudedConnection;
var connection = new Connection
{
    Type = DatabaseType.PostgreSQL,
    Server = dataSource.Server,
    Port = dataSource.Port,
    Username = dataSource.Username,
    Password = dataSource.Password,
    Database = dataSource.Database
};

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContextExtensions();
builder.Services.AddDbContextFactory<CloudedDbContext>(
    options => options.UseLazyLoadingProxies().UseNpgsql(connection.PostgresConnectionString())
);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.BackgroundBlurred = true;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddTrackingService(builder.Configuration);

builder.Services.AddScoped<AuthenticationStateProvider, CloudedAuthenticationStateProvider>();
builder.Services.Scan(
    scan =>
        scan.FromAssemblyOf<IBaseService>()
            .AddClasses(classes => classes.AssignableTo<IBaseService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
);
builder.Services.AddAutoMapper(typeof(AdminProviderMappers));

#if DEBUG
builder.Services.AddSassCompiler();
#endif

builder.Services
    .AddHealthChecks()
    .AddCheck("health/liveness", () => HealthCheckResult.Healthy())
    .AddNpgSql(connection.PostgresConnectionString(), tags: new[] { "database" });

builder.Host.UseSerilog(
    (_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
);

builder.WebHost.UseSentry(options =>
{
    options.Dsn =
        "https://036b4281a31c43b5889686bdc7364209@o4504865931919360.ingest.sentry.io/4504865963442176";
    options.EnableTracing = true;

    if (!builder.Environment.IsDevelopment())
        options.Debug = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSentryTracing();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseHealthChecks(
    "/health/liveness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/liveness") }
);
app.UseHealthChecks(
    "/health/readiness",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("database") }
);

app.Services.Migrate(typeof(CloudedDbContext));

app.Run();
