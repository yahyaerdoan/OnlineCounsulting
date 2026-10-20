using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MudBlazor.Services;
using OnlineConsulting.Maui.Shared;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Layout;
using OnlineConsulting.Maui.Web.Components;
using OnlineConsulting.Maui.Web.Infrastructure.Api;
using OnlineConsulting.Maui.Web.Infrastructure.Auth;
using OnlineConsulting.Maui.Web.Infrastructure.Hosting;
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
builder.Services.Configure<EdgeHostOptions>(builder.Configuration.GetSection("Edge"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = AppRoutes.Login;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        };

        options.Events.OnValidatePrincipal = context =>
            context.HttpContext.RequestServices.GetRequiredService<CookiePrincipalRefresher>().ValidateAsync(context);
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IRotatedTokenStore, MemoryRotatedTokenStore>();
builder.Services.AddScoped<IAccessTokenProvider, ServerAccessTokenProvider>();
builder.Services.AddScoped<IAuthSession, WebAuthSession>();
builder.Services.AddScoped<CookiePrincipalRefresher>();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https+http://api";
if (Uri.TryCreate(builder.Configuration["Api:PublicBaseUrl"], UriKind.Absolute, out var publicApiUrl))
{
    builder.Services.AddSingleton(new PublicApiOrigin(publicApiUrl));
}

builder.Services.AddMemoryCache();
builder.Services.AddTransient<TenantHostHandler>();
builder.Services.AddHttpClient(ApiHttpClientNames.Anonymous, client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<TenantHostHandler>();
// From BrowserRequestContext, not HttpContext: a circuit outlives its request.
builder.Services.AddScoped<BrowserRequestContext>();
builder.Services.AddScoped<IBrowserRequestForwarder>(sp => sp.GetRequiredService<BrowserRequestContext>());
builder.Services.AddScoped<CircuitHandler, BrowserRequestCircuitHandler>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

// First, so everything after sees the customer's domain.
app.UseMiddleware<EdgeHostMiddleware>();

// Then a business's old address (its subdomain) goes to its live custom domain.
app.UseMiddleware<CanonicalHostMiddleware>();

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

// After authentication, so it captures the signed-in user too.
app.UseMiddleware<BrowserRequestMiddleware>();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapGet(AppRoutes.Logout, async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect($"{AppRoutes.Login}?goodbye={Guid.NewGuid():N}");
});

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
