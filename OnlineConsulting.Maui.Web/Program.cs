using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using OnlineConsulting.Maui.Shared;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Layout;
using OnlineConsulting.Maui.Web.Components;
using OnlineConsulting.Maui.Web.Infrastructure.Auth;
using OnlineConsulting.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopEnd;
    config.SnackbarConfiguration.RequireInteraction = false;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
});

builder.Services.AddMauiSharedInfrastructure(typeof(App).Assembly);
builder.Services.AddSingleton<IPlatformInfo, OnlineConsulting.Maui.Web.Infrastructure.WebPlatformInfo>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = AppRoutes.Login;
        options.AccessDeniedPath = AppRoutes.Login;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAccessTokenProvider, ServerAccessTokenProvider>();
builder.Services.AddScoped<IAuthSession, WebAuthSession>();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https+http://api";
builder.Services.AddHttpClient(ApiHttpClientNames.Anonymous, client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IApiClient, ApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
    _ = app.UseHsts();
}
app.UseStatusCodePagesWithReExecute(AppRoutes.NotFound, createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapGet(AppRoutes.Logout, async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect($"{AppRoutes.Login}?goodbye={Guid.NewGuid():N}");
});

// MapRazorComponents<App> already covers App's own assembly (Maui.Web) - moduleRegistry.AdditionalAssemblies
// includes that same assembly (for Routes.razor's in-circuit Router, which has no such default), so it's
// excluded here or AddAdditionalAssemblies throws "Assembly already defined".
var moduleRegistry = app.Services.GetRequiredService<UiModuleRegistry>();
var additionalAssemblies = new[] { typeof(OnlineConsulting.Maui.Shared._Imports).Assembly }
    .Concat(moduleRegistry.AdditionalAssemblies)
    .Where(assembly => assembly != typeof(App).Assembly)
    .Distinct()
    .ToArray();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(additionalAssemblies);

app.Run();
