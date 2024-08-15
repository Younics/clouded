using System.Reflection;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Clouded.Auth.Provider;
using Clouded.Auth.Provider.DataSources;
using Clouded.Auth.Provider.DataSources.Interfaces;
using Clouded.Auth.Provider.Management.Providers;
using Clouded.Auth.Provider.Management.Services;
using Clouded.Auth.Provider.Security.Policies;
using Clouded.Auth.Provider.Services;
using Clouded.Auth.Provider.Services.Interfaces;
using Clouded.Core.DataSource.Shared;
using Clouded.Core.DataSource.Shared.Extensions;
using Clouded.Core.License;
using Clouded.Core.Mail.Library;
using Clouded.Function.Library;
using Clouded.Results.Filters;
using Clouded.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using TextCopy;
using AppContext = Clouded.Auth.Provider.AppContext;
using ApplicationOptions = Clouded.Auth.Provider.Options.ApplicationOptions;

var builder = WebApplication.CreateBuilder(args);
const string corsPolicyName = "CorsPolicy";

// Add services to the container.
builder.Services.ConfigureOptions<ApplicationOptions>(builder.Configuration);
var applicationOptions = builder.Configuration.Get<ApplicationOptions>()!;

LicenseTool.VerifyLicense(
    "Auth",
    applicationOptions.Clouded.License.Key,
    applicationOptions.Clouded.License.Text
);

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        corsPolicyName,
        policy =>
        {
            policy
                .WithOrigins(applicationOptions.Clouded.Auth.Cors.AllowedOrigins)
                .WithHeaders(applicationOptions.Clouded.Auth.Cors.AllowedHeaders)
                .WithMethods(applicationOptions.Clouded.Auth.Cors.AllowedMethods)
                .WithExposedHeaders(applicationOptions.Clouded.Auth.Cors.ExposedHeaders)
                .SetPreflightMaxAge(
                    TimeSpan.FromSeconds(applicationOptions.Clouded.Auth.Cors.MaxAge)
                );

            if (applicationOptions.Clouded.Auth.Cors.SupportsCredentials)
            {
                policy.AllowCredentials();
            }
            else
            {
                policy.DisallowCredentials();
            }
        }
    );
});

builder.Services.AddSingleton<IHashService, HashService>();
builder.Services.AddSingleton<HookResolver>(
    _ => new HookResolver(applicationOptions.Clouded.Function)
);
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserDataSource, UserDataSource>();
builder.Services.AddScoped<IDomainDataSource, DomainDataSource>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IMachineDataSource, MachineDataSource>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IOrganizationDataSource, OrganizationDataSource>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleDataSource, RoleDataSource>();
builder.Services.AddScoped<IPermissionDataSource, PermissionDataSource>();
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<ResultFilter>();
});

if (applicationOptions.Clouded.Auth.Management?.Enabled == true)
{
    builder.Services.AddScoped<
        AuthenticationStateProvider,
        ManagementAuthenticationStateProvider
    >();
    builder.Services.AddScoped<IManagementStorageService, ManagementStorageService>();
    builder.Services.AddScoped<IManagementAuthService, ManagementAuthService>();

    builder.Services.InjectClipboard();

    builder.Services.Configure<RazorPagesOptions>(
        options => options.RootDirectory = "/Management/Pages"
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
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    });
    builder.Services.AddBlazoredLocalStorage();
}

builder.Services.AddMailServices(builder.Configuration);

builder.Services
    .AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(
                new DenyAnonymousAuthorizationRequirement(),
                new BlockedAuthorizationRequirement(applicationOptions)
            )
            .Build();
    })
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenOptions = applicationOptions.Clouded.Auth.Token;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = tokenOptions.ValidateLifetime,
            ValidIssuer = tokenOptions.ValidIssuer,
            ValidateIssuer = tokenOptions.ValidateIssuer,
            ValidateAudience = tokenOptions.ValidateAudience,
            ValidateIssuerSigningKey = tokenOptions.ValidateIssuerSigningKey,
            IssuerSigningKey = tokenOptions.SigningKey
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "Clouded Auth Provider API",
            Description = "Clouded Auth Provider API",
            TermsOfService = new Uri("https://clouded.com/terms"),
            Contact = new OpenApiContact { Name = "Email", Email = "support@clouded.com" },
        }
    );

    options.SupportNonNullableReferenceTypes();

    if (builder.Environment.IsDevelopment())
    {
        options.AddSecurityDefinition(
            "CloudedKey",
            new OpenApiSecurityScheme
            {
                Name = "X-CLOUDED-KEY",
                Description = "Enter the `X-CLOUDED-KEY`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            }
        );
        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "CloudedKey"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        );
    }

    options.AddSecurityDefinition(
        "MachineKey",
        new OpenApiSecurityScheme
        {
            Name = "X-CLOUDED-MACHINE-KEY",
            Description =
                "Enter the `X-CLOUDED-MACHINE-KEY` base64 string from Machine api and secret key as following: `[API_KEY]:[SECRET_KEY]`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        }
    );
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "MachineKey"
                    }
                },
                Array.Empty<string>()
            }
        }
    );

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization string as following: `Bearer [TOKEN]`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        }
    );
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, xmlFilename));
    options.IncludeXmlComments(
        Path.Combine(System.AppContext.BaseDirectory, "Clouded.Auth.Shared.xml")
    );
});

var dataSource = applicationOptions.Clouded.DataSource;
var connection = new Connection
{
    Type = dataSource.Type,
    Server = dataSource.Server,
    Port = dataSource.Port,
    Username = dataSource.Username,
    Password = dataSource.Password,
    Database = dataSource.Database
};
var healthChecksBuilder = builder.Services
    .AddHealthChecks()
    .AddCheck("health/liveness", () => HealthCheckResult.Healthy());

switch (connection.Type)
{
    case DatabaseType.PostgreSQL:
        healthChecksBuilder.AddNpgSql(
            connection.PostgresConnectionString(),
            tags: new[] { "database" }
        );
        break;
    case DatabaseType.MySQL:
        healthChecksBuilder.AddMySql(
            connection.MysqlConnectionString(),
            tags: new[] { "database" }
        );
        break;
    default:
        throw new NotSupportedException($"Database {connection.Type} is not supported yet.");
}

builder.Host.UseSerilog(
    (_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
);

var app = builder.Build();

AppContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// Configure the HTTP request pipeline.
if (applicationOptions.Clouded.SwaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO Maybe management must be enabled
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors(corsPolicyName);
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (applicationOptions.Clouded.Auth.Management?.Enabled == true)
{
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
}

app.UseHealthChecks(
    "/health/liveness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/liveness") }
);
app.UseHealthChecks(
    "/health/readiness",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("database") }
);

app.Services.SaveSwaggerJson();

app.MigrateDatabase();

app.Run();
