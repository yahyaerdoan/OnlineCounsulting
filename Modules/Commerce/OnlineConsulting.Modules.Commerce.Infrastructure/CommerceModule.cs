using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
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
using OnlineConsulting.Modules.Commerce.Application.Features.Orders;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices;
using OnlineConsulting.Modules.Commerce.Application.Features.Invoices.Abstractions;
using OnlineConsulting.Modules.Commerce.Infrastructure.Addresses;
using OnlineConsulting.Modules.Commerce.Infrastructure.Invoices;
using OnlineConsulting.Modules.Commerce.Infrastructure.LiveUpdates;
using OnlineConsulting.Modules.Commerce.Infrastructure.Cleanup;
using OnlineConsulting.Modules.Commerce.Infrastructure.Notifications;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;
using OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Billing;
using OnlineConsulting.SharedKernel.LiveUpdates;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Commerce.Infrastructure;

public static class CommerceModule
{
    public static IServiceCollection AddCommerceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();

        _ = services.AddUserDataChangeRules(CommerceUserDataChangeRules.Configure);
        _ = services.AddDbContext<CommerceDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>())
            .AddUserDataChangeInterceptors(serviceProvider));

        _ = services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        _ = services.Configure<GeoapifyOptions>(configuration.GetSection(GeoapifyOptions.SectionName));
        _ = services.AddMemoryCache();
        _ = services.AddHttpClient(GeoapifyAddressSuggestionProvider.HttpClientName, client =>
        {
            client.BaseAddress = new Uri("https://api.geoapify.com/");
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        _ = services.AddScoped<IAddressSuggestionProvider, GeoapifyAddressSuggestionProvider>();
        _ = services.AddScoped<IBasketRepository, BasketRepository>();
        _ = services.AddScoped<IOrderRepository, OrderRepository>();
        _ = services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        _ = services.AddScoped<IEmailOutboxWriter<ICommerceOutboxModule>, EmailOutboxWriter>();
        _ = services.AddScoped<IEmailTemplate<OrderConfirmationEmailModel>, OrderConfirmationTemplate>();
        _ = services.AddScoped<IEmailTemplate<OrderPaymentFailedEmailModel>, OrderPaymentFailedTemplate>();
        _ = services.AddScoped<IEmailTemplate<OrderAbandonedEmailModel>, OrderAbandonedTemplate>();
        _ = services.AddScoped<IEmailTemplate<OrderRefundedEmailModel>, OrderRefundedTemplate>();
        _ = services.AddScoped<IOrderNotifier, OrderNotifier>();
        _ = services.AddScoped<IOrderFulfillment, OrderFulfillment>();
        _ = services.AddScoped<IEmailTemplate<InvoiceEmailModel>, InvoiceEmailTemplate>();
        _ = services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        _ = services.AddScoped<IInvoiceLineRepository, InvoiceLineRepository>();
        _ = services.AddScoped<InvoiceService>();
        _ = services.AddScoped<IInvoiceService>(sp => sp.GetRequiredService<InvoiceService>());
        _ = services.AddScoped<IServiceInvoiceIssuer>(sp => sp.GetRequiredService<InvoiceService>());
        _ = services.AddSingleton<IInvoicePdfRenderer, MigraDocInvoicePdfRenderer>();
        var business = configuration.GetSection(InvoiceBusinessInfo.SectionName).Get<InvoiceBusinessInfo>() ?? new InvoiceBusinessInfo();
        business.ClientOrigin = string.IsNullOrWhiteSpace(business.ClientOrigin) ? configuration["Auth:ClientOrigin"] : business.ClientOrigin;
        _ = services.AddSingleton(business);

        _ = services.Configure<PendingOrderCleanupOptions>(configuration.GetSection("Commerce:PendingOrderCleanup"));
        _ = services.AddHostedService<PendingOrderCleanupService>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<ICommerceTransactionRequest, CommerceDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(CommerceOperationClaims.All));
        return services;
    }
}
