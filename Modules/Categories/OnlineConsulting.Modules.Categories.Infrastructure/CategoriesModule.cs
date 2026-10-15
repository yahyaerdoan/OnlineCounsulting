using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Categories.Application.Common;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Abstractions;
using OnlineConsulting.Modules.Categories.Application;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;
using OnlineConsulting.Modules.Categories.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Categories.Infrastructure;

public static class CategoriesModule
{
    public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");


        _ = services.AddDbContext<CategoriesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<ICategoryRepository, CategoryRepository>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransactionalDbContext<ICategoriesTransactionRequest, CategoriesDbContext>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(CategoriesOperationClaims.All));
        return services;
    }
}
