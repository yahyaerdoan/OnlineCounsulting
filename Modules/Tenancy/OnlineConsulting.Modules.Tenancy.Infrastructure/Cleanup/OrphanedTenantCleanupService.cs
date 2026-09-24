using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;

/// <summary>Periodically soft-deletes abandoned Tenant rows (PendingPayment/Failed, never claimed by an admin User) left behind by ReserveTenantCommand - conservative on purpose, double-checking IUserExistenceReader when OwnerUserId is still null, since a false-positive delete is worse than clutter.</summary>
public class OrphanedTenantCleanupService(IServiceScopeFactory scopeFactory, IOptions<TenancyCleanupOptions> options, ILogger<OrphanedTenantCleanupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOnceAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Orphaned tenant cleanup cycle failed unexpectedly.");
            }

            await Task.Delay(settings.PollInterval, stoppingToken);
        }
    }

    private async Task CleanupOnceAsync(TenancyCleanupOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
        var tenantSubscriptionRepository = scope.ServiceProvider.GetRequiredService<ITenantSubscriptionRepository>();
        var tenantSubscriptionItemRepository = scope.ServiceProvider.GetRequiredService<ITenantSubscriptionItemRepository>();
        var userExistenceReader = scope.ServiceProvider.GetRequiredService<IUserExistenceReader>();

        var cutoff = DateTimeOffset.UtcNow - settings.GracePeriod;

        var candidates = await tenantRepository.GetListAsync(
            predicate: t => (t.Status == TenantStatuses.PendingPayment || t.Status == TenantStatuses.Failed) && t.CreatedDate <= cutoff,
            orderBy: q => q.OrderBy(t => t.CreatedDate),
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        if (candidates.Items.Count == 0)
        {
            return;
        }

        var reapedCount = 0;

        foreach (var tenant in candidates.Items)
        {
            if (tenant.OwnerUserId is not null)
            {
                continue;
            }

            var hasAnyUser = await userExistenceReader.AnyUserExistsForTenantAsync(tenant.Id, cancellationToken);
            if (hasAnyUser)
            {
                continue;
            }

            var subscription = await tenantSubscriptionRepository.GetAsync(
                s => s.TenantId == tenant.Id, cancellationToken: cancellationToken);

            if (subscription is not null)
            {
                var items = await tenantSubscriptionItemRepository.GetListAsync(
                    predicate: i => i.TenantSubscriptionId == subscription.Id,
                    orderBy: q => q.OrderBy(i => i.Id),
                    size: RepositoryQuerySize.Unbounded,
                    cancellationToken: cancellationToken);

                foreach (var item in items.Items)
                {
                    _ = await tenantSubscriptionItemRepository.DeleteAsync(item);
                }

                _ = await tenantSubscriptionRepository.DeleteAsync(subscription);
            }

            _ = await tenantRepository.DeleteAsync(tenant);
            reapedCount++;

            logger.LogInformation(
                "Reaped orphaned tenant {TenantId} ({TenantName}, status {TenantStatus}, created {CreatedDate:u}) - no admin user was ever created for it.",
                tenant.Id, tenant.Name, tenant.Status, tenant.CreatedDate);
        }

        if (reapedCount > 0)
        {
            logger.LogInformation("Orphaned tenant cleanup reaped {ReapedCount} of {CandidateCount} candidate tenant(s).", reapedCount, candidates.Items.Count);
        }
    }
}
