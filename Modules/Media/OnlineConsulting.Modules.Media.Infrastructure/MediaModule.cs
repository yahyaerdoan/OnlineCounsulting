using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Media.Application.Common;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Abstractions;
using OnlineConsulting.Modules.Media.Application;
using OnlineConsulting.Modules.Media.Infrastructure.Persistence;
using OnlineConsulting.Modules.Media.Infrastructure.PublicUrls;
using OnlineConsulting.Modules.Media.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Media.Infrastructure;

public static class MediaModule
{
    public static IServiceCollection AddMediaModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();

        _ = services.AddDbContext<MediaDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
        _ = services.AddScoped<IMediaAssetUrlReader, MediaAssetUrlReader>();
        _ = services.Configure<MediaPublicUrlOptions>(configuration.GetSection("Media"));

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<IMediaTransactionRequest, MediaDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(MediaOperationClaims.All));
        return services;
    }
}
