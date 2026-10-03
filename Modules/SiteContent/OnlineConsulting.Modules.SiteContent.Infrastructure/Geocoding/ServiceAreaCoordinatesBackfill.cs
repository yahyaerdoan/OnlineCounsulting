using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.Geocoding;

/// <summary>Once per startup, gives every tenant's service areas that still lack a map pin their coordinates. Areas saved from now on are
/// geocoded on save; this covers the ones that existed before and any whose lookup failed. Paced at one lookup a second (Nominatim's limit).</summary>
public sealed class ServiceAreaCoordinatesBackfill(IServiceScopeFactory scopeFactory, ILogger<ServiceAreaCoordinatesBackfill> logger) : BackgroundService
{
    private static readonly string[] TenantFilter = ["Tenant"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SiteContentDbContext>();
            var geocoder = scope.ServiceProvider.GetRequiredService<ICityGeocoder>();

            var missing = await context.ServiceAreas.IgnoreQueryFilters(TenantFilter).Where(a => a.Latitude == null).ToListAsync(stoppingToken);
            var filled = 0;
            foreach (var area in missing)
            {
                if (await geocoder.GeocodeAsync(area.Name, area.State, stoppingToken) is { } point)
                {
                    area.Latitude = point.Latitude;
                    area.Longitude = point.Longitude;
                    filled++;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(1100), stoppingToken);
            }

            if (filled > 0)
            {
                _ = await context.SaveChangesAsync(stoppingToken);
                logger.LogInformation("Added map coordinates to {Count} service area(s).", filled);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Backfilling service area coordinates failed.");
        }
    }
}
