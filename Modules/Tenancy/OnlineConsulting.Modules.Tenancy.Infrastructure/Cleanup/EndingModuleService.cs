using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.SharedKernel.FeatureFlags;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Cleanup;

/// <summary>Removes modules a tenant cancelled once their paid period ends, and turns the module off.</summary>
public class EndingModuleService(IServiceScopeFactory scopeFactory, IOptions<TenancyCleanupOptions> options, ILogger<EndingModuleService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.PollInterval);

        do
        {
            try
            {
                await EndDueModulesAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Ending module cycle failed unexpectedly.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task EndDueModulesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ITenantSubscriptionRepository>();
        var featureFlagWriter = scope.ServiceProvider.GetRequiredService<IFeatureFlagWriter>();

        var now = DateTimeOffset.UtcNow;
        var subscriptions = await subscriptionRepository.GetWithDueEndingModulesAsync(now.UtcDateTime, cancellationToken);

        foreach (var subscription in subscriptions)
        {
            var endedKeys = subscription.EndDueModules(now);
            _ = await subscriptionRepository.UpdateAsync(subscription, cancellationToken: cancellationToken);

            foreach (var moduleKey in endedKeys)
            {
                await featureFlagWriter.SetAsync(subscription.TenantId, moduleKey, false, cancellationToken);
                logger.LogInformation("Tenant {TenantId}: module {ModuleKey} ended after its paid period.", subscription.TenantId, moduleKey);
            }
        }
    }
}
