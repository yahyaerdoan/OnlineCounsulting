using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Referrals.Application;
using OnlineConsulting.Modules.Referrals.Application.Features.AccountCredits.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.ReferralCodes.Abstractions;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Abstractions;
using OnlineConsulting.Modules.Referrals.Infrastructure.Persistence;
using OnlineConsulting.Modules.Referrals.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Referrals.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Referrals.Infrastructure;

public static class ReferralsModule
{
    public static IServiceCollection AddReferralsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();
        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<ReferralsDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IReferralCodeRepository, ReferralCodeRepository>();
        _ = services.AddScoped<IReferralRepository, ReferralRepository>();
        _ = services.AddScoped<IAccountCreditRepository, AccountCreditRepository>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ReferralsTransactionAddingBehavior<,>));

        return services;
    }
}
