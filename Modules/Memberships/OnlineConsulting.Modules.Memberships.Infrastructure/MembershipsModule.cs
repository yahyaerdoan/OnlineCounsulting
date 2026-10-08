using Core.ApplicationLayer.Pipelines.Transactions.Extensions;
using Core.PersistenceLayer.Repositories.Auditing;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Memberships.Application.Common.Templates;
using OnlineConsulting.Modules.Memberships.Application;
using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Abstractions;
using OnlineConsulting.Modules.Memberships.Infrastructure.Billing;
using OnlineConsulting.Modules.Memberships.Infrastructure.LiveUpdates;
using OnlineConsulting.Modules.Memberships.Infrastructure.Cleanup;
using OnlineConsulting.Modules.Memberships.Infrastructure.Notifications;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;
using OnlineConsulting.Modules.Memberships.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.LiveUpdates;
using OnlineConsulting.SharedKernel.Memberships;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Transactions;

namespace OnlineConsulting.Modules.Memberships.Infrastructure;

public static class MembershipsModule
{
    public static IServiceCollection AddMembershipsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");


        _ = services.AddUserDataChangeRules(MembershipsUserDataChangeRules.Configure);
        _ = services.AddDbContext<MembershipsDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>())
            .AddUserDataChangeInterceptors(serviceProvider));

        _ = services.AddTransactionalDbContext<IMembershipsTransactionRequest, MembershipsDbContext>();

        _ = services.AddScoped<IMembershipPlanRepository, MembershipPlanRepository>();
        _ = services.AddScoped<ICustomerMembershipRepository, CustomerMembershipRepository>();
        _ = services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
        _ = services.AddScoped<IMemberDiscountReader, MemberDiscountReader>();
        _ = services.AddScoped<IEmailOutboxWriter<IMembershipsOutboxModule>, OnlineConsulting.Modules.Memberships.Infrastructure.Notifications.EmailOutboxWriter>();
        _ = services.AddScoped<OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.MembershipReceiptSender>();
        _ = services.AddScoped<IEmailTemplate<MembershipUpdateEmailModel>, MembershipUpdateTemplate>();
        _ = services.AddScoped<IMembershipNotifier, MembershipNotifier>();

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

        _ = services.Configure<MembershipGracePeriodOptions>(configuration.GetSection("Memberships:GracePeriod"));
        _ = services.AddHostedService<MembershipGracePeriodCleanupService>();

        _ = services.AddSingleton<IDefaultAdminPermissions>(new DefaultAdminPermissions(MembershipsOperationClaims.All));
        return services;
    }
}
