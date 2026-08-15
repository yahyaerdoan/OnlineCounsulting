using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;
using OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;
using OnlineConsulting.DataAccess.Concretions.Configurations.Extensions;
using OnlineConsulting.DataTransferObject.Concretions.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.UserInterface.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidationFilter>();
    // Suppress the framework's implicit [Required] on non-nullable reference types so FluentValidation owns validation messages.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Identity module: Identity (cookie scheme, its own default), AppIdentityDbContext, CQRS handlers.
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddUserInterfaceServiceRegistration(builder.Configuration);
builder.Services.AddBusinessLogicServiceRegistration(builder.Configuration);
builder.Services.AddDataAccesssServiceRegistration(builder.Configuration);
builder.Services.AddDataTransferObjectServiceRegistration(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminOrSuperAdminPolicy", policy => policy.RequireRole("Admin", "Super Admin"))
    .AddPolicy("RequireSuperAdminPolicy", policy => policy.RequireRole("Super Admin"))
    .AddPolicy("RequireUserDashboardPolicy", policy => policy.RequireRole("Super Admin", "Admin", "User"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//Nice error pages for error status codes (401/403/404/405/500 etc.)
app.UseStatusCodePagesWithReExecute("/errorpage/{0}");

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();
    if (!string.IsNullOrEmpty(path) &&
    context.User.Identity?.IsAuthenticated == true &&
    context.Request.Method == HttpMethods.Get &&
    !path.Contains("/account/login") &&
    !path.Contains("/account/logout") &&
    !path.Contains("/account/register"))
    {
        context.Session.SetString("last-visited-url", context.Request.Path + context.Request.QueryString);
    }

    await next();
});

app.UseNToastNotify();

//Legacy static entry point some browsers/tools still request
app.MapGet("/index.html", () => Results.Redirect("/"));

app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();
