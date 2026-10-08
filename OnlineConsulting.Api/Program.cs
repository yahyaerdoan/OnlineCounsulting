using Core.ApplicationLayer.Auditing;
using Core.ApplicationLayer.Pipelines.Authorizations.Concretions;
using Core.ApplicationLayer.Pipelines.Cachings.Concretions.CacheBehaviors;
using Core.ApplicationLayer.Pipelines.Cachings.Extensions;
using Core.ApplicationLayer.Pipelines.Loggings.Concretions;
using Core.ApplicationLayer.Pipelines.Validations.Concretions;
using Core.PersistenceLayer.MultiTenancy;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Validations;
using Core.CrossCuttingConcernLayer.ExceptionHandlings.Extensions;
using Core.SecurityLayer.Authorization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Api.LiveUpdates;
using OnlineConsulting.Modules.Categories.Application.Common;
using OnlineConsulting.Modules.Categories.Infrastructure;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Infrastructure;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Infrastructure;
using OnlineConsulting.Modules.Equipment.Infrastructure.Persistence;
using OnlineConsulting.Modules.FeatureFlags.Application.Common;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure.Persistence;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Constants;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.Modules.Identity.Infrastructure.Persistence;
using OnlineConsulting.Modules.Identity.Infrastructure.Bootstrapping;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using OnlineConsulting.Modules.Inquiries.Infrastructure;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.Modules.Media.Application.Common;
using OnlineConsulting.Modules.Media.Infrastructure;
using OnlineConsulting.Modules.Media.Infrastructure.Persistence;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Infrastructure;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;
using OnlineConsulting.Modules.Referrals.Application.Common;
using OnlineConsulting.Modules.Referrals.Infrastructure;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Infrastructure;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Infrastructure;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Infrastructure;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;
using OnlineConsulting.Modules.Tenancy.Infrastructure;
using OnlineConsulting.Modules.Tenancy.Infrastructure.Persistence;
using OnlineConsulting.Notifications;
using OnlineConsulting.Notifications.Persistence;
using OnlineConsulting.Payments;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.SharedKernel.LiveUpdates;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.Storage;
using Scalar.AspNetCore;

FriendlyValidationMessages.Apply();

var builder = WebApplication.CreateBuilder(args);

// The ASP.NET Core dev cert's SAN list has no entry for the Android emulator's host alias
// (10.0.2.2), so https to it fails TLS hostname verification for anything a WebView loads
// directly (img/iframe src). Connections by IP (the emulator; no SNI hostname) get the DevCerts
// cert whose SAN covers 10.0.2.2; connections by name (localhost: browsers, Aspire, maui-web)
// keep the trusted ASP.NET Core dev cert. See DevCerts/README.
builder.UseEmulatorCertificateWhenPresent();

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuditing();
builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());
builder.Services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantProvider>());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantStatusCheckBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IValidator<>), typeof(DynamicListRequestValidator<>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogResultAddingBehavior<,>));

var redisConnection = builder.Configuration.GetConnectionString("Redis");

_ = string.IsNullOrWhiteSpace(redisConnection)
    ? builder.Services.AddDistributedMemoryCache()
    : builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);

builder.Services.AddCacheSettings(builder.Configuration);
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
    ["Commerce"] = CommerceOperationClaims.All,
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
builder.Services.AddSingleton<IUserDataChangePublisher, SignalRUserDataChangePublisher>();
builder.Services.AddPaymentsInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<CategoriesDbContext>()
    .AddDbContextCheck<CommerceDbContext>()
    .AddDbContextCheck<EquipmentDbContext>()
    .AddDbContextCheck<FeatureFlagsDbContext>()
    .AddDbContextCheck<AppIdentityDbContext>()
    .AddDbContextCheck<InquiriesDbContext>()
    .AddDbContextCheck<MediaDbContext>()
    .AddDbContextCheck<MembershipsDbContext>()
    .AddDbContextCheck<ReferralsDbContext>()
    .AddDbContextCheck<SchedulingDbContext>()
    .AddDbContextCheck<ServicesDbContext>()
    .AddDbContextCheck<SiteContentDbContext>()
    .AddDbContextCheck<TenancyDbContext>()
    .AddDbContextCheck<NotificationsDbContext>();

builder.Services.AddApiServiceRegistration(builder.Environment);
builder.Services.AddApiJson();

// KnownNetworks/KnownProxies cleared - proxy IP isn't known ahead of deployment; without this
// RemoteIpAddress (used for rate-limit partitioning) always resolves to the proxy, not the client.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

await RoleBootstrapper.EnsureAsync(app.Services);
await SuperAdminBootstrapper.EnsureAsync(app.Services);

app.MapDefaultEndpoints();

app.UseConfigureCustomExceptionMiddleware();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
    _ = app.MapScalarApiReference();
}

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health") && !context.Request.Path.StartsWithSegments("/alive"),
    branch => branch.UseHttpsRedirection());
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TenantHostMiddleware>();
app.UseAuthorization();
app.UseRateLimiter();

app.MapEndpoints();
app.MapHub<TechnicianTrackingHub>("/hubs/technician-tracking");
app.MapHub<UserUpdatesHub>(UserUpdatesHub.Path);

app.Run();
