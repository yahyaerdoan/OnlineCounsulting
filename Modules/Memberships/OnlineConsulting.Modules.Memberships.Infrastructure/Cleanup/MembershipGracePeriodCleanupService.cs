using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Cleanup;

/// <summary>Auto-cancels a CustomerMembership that's been PastDue longer than the configured grace
/// period - mirrors Commerce's PendingOrderCleanupService.</summary>
public class MembershipGracePeriodCleanupService(IServiceScopeFactory scopeFactory, IOptions<MembershipGracePeriodOptions> options, ISubscriptionGateway subscriptionGateway, ILogger<MembershipGracePeriodCleanupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        using var timer = new PeriodicTimer(settings.PollInterval);

        do
        {
            try
            {
                await CleanupOnceAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Membership grace-period cleanup cycle failed unexpectedly.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CleanupOnceAsync(MembershipGracePeriodOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var membershipRepository = scope.ServiceProvider.GetRequiredService<ICustomerMembershipRepository>();

        var cutoff = DateTimeOffset.UtcNow - settings.GraceAfter;

        var candidates = await membershipRepository
            .GetAllAsync(predicate: m => m.Status == CustomerMembershipStatuses.PastDue && m.PastDueSince != null && m.PastDueSince <= cutoff, orderBy: q => q.OrderBy(m => m.PastDueSince), cancellationToken: cancellationToken);

        if (candidates.Count == 0)
        {
            return;
        }

        var notifier = scope.ServiceProvider.GetRequiredService<IMembershipNotifier>();
        var cancelledCount = 0;

        foreach (var membership in candidates)
        {
            if (membership.ProviderSubscriptionId is not null)
            {
                _ = await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, cancellationToken: cancellationToken);
            }

            membership.Cancel();

            _ = await membershipRepository.UpdateAsync(membership, cancellationToken: cancellationToken);

            cancelledCount++;

            await notifier.CancelledAfterFailuresAsync(membership, cancellationToken);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Membership grace-period cleanup cancelled {CancelledCount} of {CandidateCount} PastDue membership(s).", cancelledCount, candidates.Count);
        }
    }
}
