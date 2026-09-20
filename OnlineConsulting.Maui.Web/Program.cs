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
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        // AccessDeniedPath defaulted to the login page - a signed-in user with the wrong role
        // (e.g. an admin opening the Customer-only dashboard) landed back on the login form
        // instead of just being sent home. This only fires for a fresh top-level request (the
        // page component's own [Authorize] is also endpoint metadata, enforced here before Blazor
        // even renders); an in-circuit SPA navigation never hits this and goes through
        // RedirectToLogin.razor instead, which already does the right thing.
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        };

        // Runs on a real top-level request, before Blazor's circuit takes over - the one point
        // where a cookie rewrite is possible. Keeps role/IsSuperAdmin claims fresh via the same
        // refresh-token flow that already renews the Api access token, instead of leaving them
        // frozen at login-time until the user manually signs out and back in.
        options.Events.OnValidatePrincipal = context =>
            context.HttpContext.RequestServices.GetRequiredService<CookiePrincipalRefresher>().ValidateAsync(context);
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAccessTokenProvider, ServerAccessTokenProvider>();
builder.Services.AddScoped<IAuthSession, WebAuthSession>();
builder.Services.AddScoped<CookiePrincipalRefresher>();

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "https+http://api";
builder.Services.AddHttpClient(ApiHttpClientNames.Anonymous, client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddTransient<GuestIdHandler>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<GuestIdHandler>();

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

// App's own assembly is excluded here - already covered by MapRazorComponents<App>, else AddAdditionalAssemblies throws.
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
