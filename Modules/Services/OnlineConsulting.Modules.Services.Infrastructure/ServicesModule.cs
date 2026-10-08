using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.Abstractions;
using OnlineConsulting.Modules.Services.Application;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.Modules.Services.Infrastructure.Catalog;
using OnlineConsulting.Modules.Services.Infrastructure.Persistence;
using OnlineConsulting.Modules.Services.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Catalog;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Services.Infrastructure;

public static class ServicesModule
{
    public static IServiceCollection AddServicesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");


        _ = services.AddDbContext<ServicesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IServiceRepository, ServiceRepository>();
        _ = services.AddScoped<IServiceCatalogReader, ServiceCatalogReader>();
        _ = services.AddScoped<IServiceMediaItemRepository, ServiceMediaItemRepository>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<IServicesTransactionRequest, ServicesDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(ServicesOperationClaims.All));
        return services;
    }
}
