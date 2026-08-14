using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Commerce.Application;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.Modules.Commerce.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure;

public static class CommerceModule
{
    public static IServiceCollection AddCommerceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();

        services.AddDbContext<CommerceDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>()));

        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommerceTransactionAddingBehavior<,>));

        return services;
    }
}
