using System.Text.Json.Serialization;
using Clouded.Platform.Provider.Options;
using Clouded.Platform.Provider.Services;
using Clouded.Platform.Provider.Services.Interfaces;
using Clouded.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureOptions<ApplicationOptions>(builder.Configuration);
var applicationOptions = builder.Configuration.Get<ApplicationOptions>();

builder.Services.Configure<JsonOptions>(
    o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
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
            ValidAudience = tokenOptions.ValidAudience,
            ValidateAudience = tokenOptions.ValidateAudience,
            ValidateIssuerSigningKey = tokenOptions.ValidateIssuerSigningKey,
            IssuerSigningKey = tokenOptions.SigningKey
        };
    });

// Add services to the container.
builder.Services.AddSingleton<IHubService, HubService>();
builder.Services.AddSingleton<IKubernetesService, KubernetesService>();
builder.Services.AddSingleton<IProviderService, ProviderService>();
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddHostedService<BackgroundNamespacesWatcherService>();
builder.Services.AddHostedService<BackgroundPodsWatcherService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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
});

builder.Services
    .AddHealthChecks()
    .AddCheck("health/liveness", () => HealthCheckResult.Healthy())
    .AddCheck("health/readiness", () => HealthCheckResult.Healthy());

builder.Host.UseSerilog(
    (_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks(
    "/health/liveness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/liveness") }
);
app.UseHealthChecks(
    "/health/readiness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/readiness") }
);

await app.Services.GetRequiredService<IHubService>().InitializeAsync();

app.Run();
