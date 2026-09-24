using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Commerce.Application;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Common.Templates;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Infrastructure.Cleanup;
using OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.Modules.Commerce.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Infrastructure;

public static class CommerceModule
{
    public static IServiceCollection AddCommerceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();
        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<CommerceDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        _ = services.AddScoped<IBasketRepository, BasketRepository>();
        _ = services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        _ = services.AddScoped<IOrderRepository, OrderRepository>();
        _ = services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        _ = services.AddScoped<IEmailOutboxWriter<ICommerceOutboxModule>, EmailOutboxWriter>();
        _ = services.AddScoped<IEmailTemplate<OrderConfirmationEmailModel>, OrderConfirmationTemplate>();
        _ = services.AddScoped<IEmailTemplate<OrderPaymentFailedEmailModel>, OrderPaymentFailedTemplate>();
        _ = services.AddScoped<IEmailTemplate<OrderAbandonedEmailModel>, OrderAbandonedTemplate>();

        _ = services.Configure<PendingOrderCleanupOptions>(configuration.GetSection("Commerce:PendingOrderCleanup"));
        _ = services.AddHostedService<PendingOrderCleanupService>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommerceTransactionAddingBehavior<,>));

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(CommerceOperationClaims.All));
        return services;
    }
}
