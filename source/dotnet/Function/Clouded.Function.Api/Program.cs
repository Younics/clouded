using System.Text.Json.Serialization;
using Clouded.Function.Api.Options;
using Clouded.Function.Library.Services;
using Clouded.Shared;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureOptions<ApplicationOptions>(builder.Configuration);
builder.Services.Configure<JsonOptions>(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSingleton<IFunctionService, FunctionService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "CloudedKey",
        new OpenApiSecurityScheme
        {
            Name = "X-CLOUDED-KEY",
            Description = "Enter the `X-CLOUDED-KEY` string",
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

app.MapControllers();

app.UseHealthChecks(
    "/health/liveness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/liveness") }
);
app.UseHealthChecks(
    "/health/readiness",
    new HealthCheckOptions { Predicate = r => r.Name.Contains("/health/readiness") }
);

app.Run();
