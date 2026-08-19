using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Identity;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;

/// <summary>Periodically soft-deletes Tenant rows (plus their TenantSubscription/TenantSubscriptionItem rows) that ReserveTenantCommand left behind in PendingPayment/Failed and that were never claimed by an admin User - i.e. a signup that was abandoned before CreateTenantAdminCommand/SetTenantOwnerCommand ever ran (browser closed, network error, user never retried). Harmless clutter otherwise, since such a tenant can never legitimately progress out of PendingPayment/Failed. Mirrors OutboxDispatcher's periodic BackgroundService shape (see OnlineConsulting.Notifications.Dispatch.OutboxDispatcher) - same poll-loop-with-its-own-scope structure, just no per-row retry/backoff bookkeeping since a "reap" here is a single soft-delete, not a fallible external call.
/// Deliberately conservative: only tenants older than TenancyCleanupOptions.GracePeriod are considered, and the orphan signal is two-layered - Tenant.OwnerUserId is the primary, cheap signal (set by SetTenantOwnerCommand right after CreateTenantAdminCommand, before any billing - see SignUp.cs), but a tenant with OwnerUserId still null is also re-checked against IUserExistenceReader (cross-module) before deleting, to cover the narrow window where CreateTenantAdminCommand already created the User but the process crashed before the following SetTenantOwnerCommand step persisted the owner. A false positive here (deleting a tenant that's actually still live) is worse than leaving clutter behind.</summary>
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
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        if (candidates.Items.Count == 0)
            return;

        var reapedCount = 0;

        foreach (var tenant in candidates.Items)
        {
            if (tenant.OwnerUserId is not null)
                continue;

            var hasAnyUser = await userExistenceReader.AnyUserExistsForTenantAsync(tenant.Id, cancellationToken);
            if (hasAnyUser)
                continue;

            var subscription = await tenantSubscriptionRepository.GetAsync(
                s => s.TenantId == tenant.Id, cancellationToken: cancellationToken);

            if (subscription is not null)
            {
                var items = await tenantSubscriptionItemRepository.GetListAsync(
                    predicate: i => i.TenantSubscriptionId == subscription.Id,
                    size: RepositoryQuerySize.Unbounded,
                    cancellationToken: cancellationToken);

                foreach (var item in items.Items)
                    await tenantSubscriptionItemRepository.DeleteAsync(item);

                await tenantSubscriptionRepository.DeleteAsync(subscription);
            }

            await tenantRepository.DeleteAsync(tenant);
            reapedCount++;

            logger.LogInformation(
                "Reaped orphaned tenant {TenantId} ({TenantName}, status {TenantStatus}, created {CreatedDate:u}) - no admin user was ever created for it.",
                tenant.Id, tenant.Name, tenant.Status, tenant.CreatedDate);
        }

        if (reapedCount > 0)
            logger.LogInformation("Orphaned tenant cleanup reaped {ReapedCount} of {CandidateCount} candidate tenant(s).", reapedCount, candidates.Items.Count);
    }
}
