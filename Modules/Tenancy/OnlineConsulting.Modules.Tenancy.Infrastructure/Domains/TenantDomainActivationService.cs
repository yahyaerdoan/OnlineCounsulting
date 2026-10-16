using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>Activates pending tenant domains once ready; drops ones still pending after the timeout.</summary>
public class TenantDomainActivationService(IServiceScopeFactory scopeFactory, IOptions<TenantDomainOptions> options, ILogger<TenantDomainActivationService> logger)
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
                await CheckOnceAsync(settings, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Tenant domain check cycle failed unexpectedly.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CheckOnceAsync(TenantDomainOptions settings, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var domainRepository = scope.ServiceProvider.GetRequiredService<ITenantDomainRepository>();
        var gateway = scope.ServiceProvider.GetRequiredService<ICustomDomainGateway>();
        var activator = scope.ServiceProvider.GetRequiredService<TenantDomainActivator>();

        var pending = await domainRepository.GetAllAsync(predicate: d => d.Status == TenantDomainStatuses.Pending, cancellationToken: cancellationToken);
        var cutoff = DateTimeOffset.UtcNow - settings.PendingTimeout;

        foreach (var domain in pending)
        {
            try
            {
                if (domain.CreatedDate <= cutoff)
                {
                    await gateway.RemoveAsync(domain.ProviderDomainId, cancellationToken);
                    _ = await domainRepository.DeleteAsync(domain, cancellationToken: cancellationToken);
                    logger.LogInformation("Removed tenant domain {Host} of tenant {TenantId}; still pending after {Timeout}.", domain.Host, domain.TenantId, settings.PendingTimeout);
                    continue;
                }

                _ = await activator.SyncAsync(domain, cancellationToken);
                if (domain.IsActive)
                {
                    logger.LogInformation("Activated tenant domain {Host} of tenant {TenantId}.", domain.Host, domain.TenantId);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(ex, "Checking tenant domain {Host} of tenant {TenantId} failed.", domain.Host, domain.TenantId);
            }
        }
    }
}
