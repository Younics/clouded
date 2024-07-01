using BlazorComponentBus;
using Blazored.LocalStorage;
using Clouded.Admin.Provider;
using Clouded.Admin.Provider.DataSources;
using Clouded.Admin.Provider.DataSources.Interfaces;
using Clouded.Admin.Provider.Options;
using Clouded.Admin.Provider.Providers;
using Clouded.Admin.Provider.Services;
using Clouded.Admin.Provider.Services.Interfaces;
using Clouded.Function.Library.Services;
using Clouded.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using AppContext = Clouded.Admin.Provider.AppContext;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.ConfigureOptions<ApplicationOptions>(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<ComponentBus>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IUserSettingsDataSource, UserSettingsDataSource>();
builder.Services.AddScoped<AuthenticationStateProvider, AdminAuthenticationStateProvider>();

builder.Host.UseSerilog(
    (_, configuration) => configuration.ReadFrom.Configuration(builder.Configuration)
);

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

AppContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MigrateDatabase();

app.Run();
