using Core.ApplicationLayer.Pipelines.Authorizations.Concretions;
using Core.ApplicationLayer.Pipelines.Cachings.Concretions.CacheBehaviors;
using Core.ApplicationLayer.Pipelines.Loggings.Concretions;
using Core.ApplicationLayer.Pipelines.Validations.Concretions;
using Core.CrossCuttingConcernLayer.ExceptionHandlings.Extensions;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Categories.Infrastructure;
using OnlineConsulting.Modules.Commerce.Infrastructure;
using OnlineConsulting.Modules.FeatureFlags.Infrastructure;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.Modules.Identity.Infrastructure.Seeding;
using OnlineConsulting.Modules.Inquiries.Infrastructure;
using OnlineConsulting.Modules.Media.Infrastructure;
using OnlineConsulting.Modules.Scheduling.Infrastructure;
using OnlineConsulting.Modules.Services.Infrastructure;
using OnlineConsulting.Modules.SiteContent.Infrastructure;
using OnlineConsulting.Notifications;
using OnlineConsulting.Payments;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogResultAddingBehavior<,>));

var redisConnection = builder.Configuration.GetConnectionString("Redis");

if (string.IsNullOrWhiteSpace(redisConnection))
    builder.Services.AddDistributedMemoryCache();
else
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheRemovingBehavior<,>));

builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddCommerceModule(builder.Configuration);
builder.Services.AddFeatureFlagsModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration).AddIdentityModuleJwtBearer(builder.Configuration);
builder.Services.AddServicesModule(builder.Configuration);
builder.Services.AddInquiriesModule(builder.Configuration);
builder.Services.AddSchedulingModule(builder.Configuration);
builder.Services.AddSiteContentModule(builder.Configuration);
builder.Services.AddMediaModule(builder.Configuration);
builder.Services.AddStorageInfrastructure(builder.Configuration);
builder.Services.PostConfigure<StorageOptions>(options =>
{
    if (string.IsNullOrEmpty(options.Local.RootPath))
        options.Local.RootPath = Path.Combine(builder.Environment.WebRootPath, "media");
});
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddPaymentsInfrastructure(builder.Configuration);

builder.Services.AddApiServiceRegistration(builder.Configuration);

var app = builder.Build();

await RoleSeeder.SeedAsync(app.Services);

app.MapDefaultEndpoints();

app.UseConfigureCustomExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
