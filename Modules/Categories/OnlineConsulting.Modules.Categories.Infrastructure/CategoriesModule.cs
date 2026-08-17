using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Categories.Application;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using OnlineConsulting.Modules.Categories.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Categories.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Infrastructure;

public static class CategoriesModule
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<CategoriesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CategoriesTransactionAddingBehavior<,>));

        return services;
    }
}
