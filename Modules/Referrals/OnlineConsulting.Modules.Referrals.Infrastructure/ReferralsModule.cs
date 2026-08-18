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
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<ReferralsDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IReferralCodeRepository, ReferralCodeRepository>();
        services.AddScoped<IReferralRepository, ReferralRepository>();
        services.AddScoped<IAccountCreditRepository, AccountCreditRepository>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ReferralsTransactionAddingBehavior<,>));

        return services;
    }
}
