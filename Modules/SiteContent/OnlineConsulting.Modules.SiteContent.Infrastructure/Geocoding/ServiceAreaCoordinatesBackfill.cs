using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Geocoding;

/// <summary>Once per startup, gives every tenant's service areas that still lack a map pin their coordinates. Areas saved from now on are
/// geocoded on save; this covers the ones that existed before and any whose lookup failed. Paced at one lookup a second (Nominatim's limit).
/// Reads across tenants, then saves each tenant's areas inside its own TenantScope, since the save guard refuses cross-tenant writes.</summary>
public sealed class ServiceAreaCoordinatesBackfill(IServiceScopeFactory scopeFactory, ILogger<ServiceAreaCoordinatesBackfill> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SiteContentDbContext>();
            var geocoder = scope.ServiceProvider.GetRequiredService<ICityGeocoder>();

            var missing = await context.ServiceAreas.IgnoreTenantFilter().Where(a => a.Latitude == null).ToListAsync(stoppingToken);
            var filled = 0;
            foreach (var tenantAreas in missing.GroupBy(a => a.TenantId))
            {
                using var tenantScope = TenantScope.Begin(tenantAreas.Key);
                var filledForTenant = 0;

                foreach (var area in tenantAreas)
                {
                    if (await geocoder.GeocodeAsync(area.Name, area.State, stoppingToken) is { } point)
                    {
                        area.Latitude = point.Latitude;
                        area.Longitude = point.Longitude;
                        filledForTenant++;
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(1100), stoppingToken);
                }

                if (filledForTenant > 0)
                {
                    _ = await context.SaveChangesAsync(stoppingToken);
                    filled += filledForTenant;
                }
            }

            if (filled > 0)
            {
                logger.LogInformation("Added map coordinates to {Count} service area(s).", filled);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Backfilling service area coordinates failed.");
        }
    }
}
