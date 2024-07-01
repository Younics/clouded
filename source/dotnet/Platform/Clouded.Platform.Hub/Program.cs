using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Platform.Database;
using Clouded.Platform.Database.Contexts;
using Clouded.Platform.Hub.Options;
using Clouded.Platform.Hub.Provider;
using Clouded.Platform.Hub.Security;
using Clouded.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddDbContextExtensions();
builder.Services.AddDbContextFactory<CloudedDbContext>(
    options => options.UseLazyLoadingProxies().UseNpgsql(connection.PostgresConnectionString())
);

builder.Services
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenOptions = applicationOptions.Clouded.Auth.Token;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = tokenOptions.ValidIssuer,
            ValidateIssuer = tokenOptions.ValidateIssuer,
            ValidAudiences = tokenOptions.ValidAudiences,
            ValidateAudience = tokenOptions.ValidateAudience,
            ValidateIssuerSigningKey = tokenOptions.ValidateIssuerSigningKey,
            IssuerSigningKey = tokenOptions.SigningKey
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Request.Query.TryGetValue("access_token", out var accessToken);

                if (
                    !StringValues.IsNullOrEmpty(accessToken)
                    && context.HttpContext.Request.Path.StartsWithSegments("/hubs")
                )
                    context.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<CloudedAuthorizeFilter>();

builder.Services.AddControllers();
builder.Services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, UserIdProvider>();
builder.Services
    .AddSignalR()
    .AddJsonProtocol()
    .AddHubOptions<ProviderHub>(options =>
    {
        options.EnableDetailedErrors = true;
    });
builder.Services.AddResponseCompression(
    opts =>
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" }
        )
);

builder.Services
    .AddHealthChecks()
    .AddCheck("health/liveness", () => HealthCheckResult.Healthy())
    .AddNpgSql(connection.PostgresConnectionString(), tags: new[] { "database" });

builder.Host.UseSerilog(
    (_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
);

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ProviderHub>("/hubs/provider");
});

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
