using Core.ApplicationLayer.Pipelines.Authorizations.Concretions;
using Core.ApplicationLayer.Pipelines.Transactions.Concretions;
using Core.ApplicationLayer.Pipelines.Validations.Concretions;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Infrastructure;
using OnlineConsulting.Modules.Categories.Infrastructure;
using OnlineConsulting.SharedKernel.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionAddingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationAddingBehavior<,>));

// Module registrations go before AddApiServiceRegistration's Scrutor convention scan, so its
// RegistrationStrategy.Skip sees each module's repositories already registered and doesn't
// double-register them.
builder.Services.AddCategoriesModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration).AddIdentityModuleJwtBearer(builder.Configuration);

builder.Services.AddApiServiceRegistration(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

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
