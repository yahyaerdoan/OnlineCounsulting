using Core.ApplicationLayer.Pipelines.Authorizations.Concretions;
using Core.ApplicationLayer.Pipelines.Cachings.Concretions.CacheBehaviors;
using Core.ApplicationLayer.Pipelines.Loggings.Concretions;
using Core.ApplicationLayer.Pipelines.Validations.Concretions;
using Core.CrossCuttingConcernLayer.ExceptionHandlings.Extensions;
using Core.SecurityLayer.Authorization;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Api.Seeding;
using OnlineConsulting.Modules.Categories.Application.Features.Constants;
using OnlineConsulting.Modules.Categories.Infrastructure;
using OnlineConsulting.Modules.Commerce.Infrastructure;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Infrastructure;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.Modules.Identity.Infrastructure.Seeding;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using OnlineConsulting.Modules.Inquiries.Infrastructure;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using OnlineConsulting.Modules.Media.Infrastructure;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Infrastructure;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Infrastructure;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Infrastructure;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Infrastructure;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Infrastructure;
using OnlineConsulting.Modules.Tenancy.Infrastructure;
using OnlineConsulting.Notifications;
using OnlineConsulting.Payments;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Validation;
using OnlineConsulting.Storage;

FriendlyValidationMessages.Apply();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantStatusCheckBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogResultAddingBehavior<,>));

var redisConnection = builder.Configuration.GetConnectionString("Redis");

_ = string.IsNullOrWhiteSpace(redisConnection)
    ? builder.Services.AddDistributedMemoryCache()
    : builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheRemovingBehavior<,>));

builder.Services.AddSingleton<IPermissionCatalog>(new PermissionCatalog(new Dictionary<string, string[]>
{
    ["Categories"] = CategoriesOperationClaims.All,
    ["FeatureFlags"] = FeatureFlagsOperationClaims.All,
    ["Roles"] = RolesOperationClaims.All,
    ["Users"] = UsersOperationClaims.All,
    ["Invites"] = InvitesOperationClaims.All,
    ["Contact"] = ContactOperationClaims.All,
    ["Messages"] = MessagesOperationClaims.All,
    ["Newsletter"] = NewsletterOperationClaims.All,
    ["Media"] = MediaOperationClaims.All,
    ["Scheduling"] = SchedulingOperationClaims.All,
    ["Services"] = ServicesOperationClaims.All,
    ["SiteContent"] = SiteContentOperationClaims.All,
    ["Memberships"] = MembershipsOperationClaims.All,
    ["Referrals"] = ReferralsOperationClaims.All,
    ["Equipment"] = EquipmentOperationClaims.All,
}));

builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddCommerceModule(builder.Configuration);
builder.Services.AddFeatureFlagsModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration).AddIdentityModuleJwtBearer(builder.Configuration);
builder.Services.AddServicesModule(builder.Configuration);
builder.Services.AddInquiriesModule(builder.Configuration);
builder.Services.AddSchedulingModule(builder.Configuration);
builder.Services.AddSiteContentModule(builder.Configuration);
builder.Services.AddMediaModule(builder.Configuration);
builder.Services.AddMembershipsModule(builder.Configuration);
builder.Services.AddReferralsModule(builder.Configuration);
builder.Services.AddEquipmentModule(builder.Configuration);
builder.Services.AddTenancyModule(builder.Configuration);
builder.Services.AddStorageInfrastructure(builder.Configuration);
builder.Services.PostConfigure<StorageOptions>(options =>
{
    if (string.IsNullOrEmpty(options.Local.RootPath))
    {
        options.Local.RootPath = Path.Combine(builder.Environment.WebRootPath, "media");
    }
});
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddPaymentsInfrastructure(builder.Configuration);

builder.Services.AddApiServiceRegistration(builder.Configuration);

// KnownNetworks/KnownProxies cleared because the reverse proxy's IP isn't known ahead of deployment
// (nginx/k8s ingress/Azure App Service front end) - trusting the network boundary instead, since only
// that proxy can reach this app. Without this, RemoteIpAddress (used for anonymous rate-limit
// partitioning - see ServiceRegistration.GetPartitionKey) always resolves to the proxy, not the client.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

await RoleSeeder.SeedAsync(app.Services);
await HvacCatalogSeeder.SeedAsync(app.Services);
await SuperAdminSeeder.SeedAsync(app.Services);

app.MapDefaultEndpoints();

app.UseConfigureCustomExceptionMiddleware();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapEndpoints();
app.MapHub<TechnicianTrackingHub>("/hubs/technician-tracking");

app.Run();
