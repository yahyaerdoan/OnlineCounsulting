using Core.ApplicationLayer.Pipelines.Authorizations.Concretions;
using Core.ApplicationLayer.Pipelines.Validations.Concretions;
using Core.CrossCuttingConcernLayer.ExceptionHandlings.Extensions;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Categories.Infrastructure;
using OnlineConsulting.Modules.Commerce.Infrastructure;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.Modules.Inquiries.Infrastructure;
using OnlineConsulting.Modules.Scheduling.Infrastructure;
using OnlineConsulting.Modules.Services.Infrastructure;
using OnlineConsulting.Notifications;
using OnlineConsulting.ServiceDefaults;
using OnlineConsulting.SharedKernel.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationAddingBehavior<,>));

// Module registrations must precede AddApiServiceRegistration's Scrutor convention scan, so RegistrationStrategy.Skip sees each module's repositories already registered and doesn't double-register them.
builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddCommerceModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration).AddIdentityModuleJwtBearer(builder.Configuration);
builder.Services.AddServicesModule(builder.Configuration);
builder.Services.AddInquiriesModule(builder.Configuration);
builder.Services.AddSchedulingModule(builder.Configuration);
builder.Services.AddNotificationsInfrastructure(builder.Configuration);

builder.Services.AddApiServiceRegistration(builder.Configuration);

var app = builder.Build();

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
