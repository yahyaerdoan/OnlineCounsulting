using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;
using OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;
using OnlineConsulting.DataAccess.Concretions.Configurations.Extensions;
using OnlineConsulting.DataAccess.Concretions.Contexts;
using OnlineConsulting.DataTransferObject.Concretions.Configurations.Extensions;
using OnlineConsulting.Entity.Concretions.Entities;
using OnlineConsulting.UserInterface.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Add MVC + custom validation filter + global authorize policy
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ValidationFilter>();
    //Don't let non-nullable reference types generate an implicit [Required] with the
    //framework's default "The {0} field is required." message - let FluentValidation own it.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Add Identity with EF store
builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<OnlineConsultingDbContext>().AddDefaultTokenProviders();

//Register your custom DI layers
builder.Services.AddUserInterfaceServiceRegistration(builder.Configuration);
builder.Services.AddBusinessLogicServiceRegistration(builder.Configuration);
builder.Services.AddDataAccesssServiceRegistration(builder.Configuration);
builder.Services.AddDataTransferObjectServiceRegistration(builder.Configuration);

//Custom authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminOrSuperAdminPolicy", policy => policy.RequireRole("Admin", "Super Admin"))
    .AddPolicy("RequireSuperAdminPolicy", policy => policy.RequireRole("Super Admin"))
    .AddPolicy("RequireUserDashboardPolicy", policy => policy.RequireRole("Super Admin", "Admin", "User"));

var app = builder.Build();

//Error handling for production
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

//Authentication & Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

//Short inline middleware: store LastVisitedUrl
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

//Your toast notification service
app.UseNToastNotify();

//Legacy static entry point some browsers/tools still request
app.MapGet("/index.html", () => Results.Redirect("/"));

//Routing: areas + default route
app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();
