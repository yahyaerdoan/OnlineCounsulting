using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Categories.Application;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using OnlineConsulting.Modules.Categories.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Infrastructure;

public static class CategoriesModule
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();

        services.AddDbContext<CategoriesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));

        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ICategoryRepository).Assembly));
        services.AddValidatorsFromAssembly(typeof(ICategoryRepository).Assembly);

        return services;
    }
}
