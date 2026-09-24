using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Constants;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Persistence;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Cleanup;

/// <summary>Auto-cancels a CustomerMembership that's been PastDue longer than the configured grace
/// period - mirrors Commerce's PendingOrderCleanupService.</summary>
public class MembershipGracePeriodCleanupService(IServiceScopeFactory scopeFactory, IOptions<MembershipGracePeriodOptions> options, ISubscriptionGateway subscriptionGateway, ILogger<MembershipGracePeriodCleanupService> logger)
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
                logger.LogError(ex, "Membership grace-period cleanup cycle failed unexpectedly.");
            }

            await Task.Delay(settings.PollInterval, stoppingToken);
        }
    }

    private async Task CleanupOnceAsync(MembershipGracePeriodOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var membershipRepository = scope.ServiceProvider.GetRequiredService<ICustomerMembershipRepository>();

        var cutoff = DateTimeOffset.UtcNow - settings.GraceAfter;

        var candidates = await membershipRepository
            .GetListAsync(predicate: m => m.Status == CustomerMembershipStatuses.PastDue && m.PastDueSince != null && m.PastDueSince <= cutoff, orderBy: q => q.OrderBy(m => m.PastDueSince), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        if (candidates.Items.Count == 0)
        {
            return;
        }

        var pushNotificationSender = scope.ServiceProvider.GetRequiredService<IPushNotificationSender>();
        var cancelledCount = 0;

        foreach (var membership in candidates.Items)
        {
            if (membership.ProviderSubscriptionId is not null)
            {
                _ = await subscriptionGateway.CancelSubscriptionAsync(membership.ProviderSubscriptionId, cancellationToken: cancellationToken);
            }

            membership.Status = CustomerMembershipStatuses.Cancelled;
            membership.PastDueSince = null;

            _ = await membershipRepository.UpdateAsync(membership);

            cancelledCount++;

            await pushNotificationSender.SendToUserAsync(membership.UserId,
                "Membership cancelled", "Your membership was cancelled after repeated payment failures. Subscribe again anytime to restore your benefits.",
                new Dictionary<string, string> { ["customerMembershipId"] = membership.Id.ToString() }, cancellationToken);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Membership grace-period cleanup cancelled {CancelledCount} of {CandidateCount} PastDue membership(s).", cancelledCount, candidates.Items.Count);
        }
    }
}
