using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.SiteContent.Infrastructure.RichText;

/// <summary>Once per startup, sanitizes About Us descriptions saved before sanitizing on save existed. Saves each tenant inside its own TenantScope.</summary>
public sealed class AboutUsHtmlBackfill(IServiceScopeFactory scopeFactory, ILogger<AboutUsHtmlBackfill> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SiteContentDbContext>();
            var sanitizer = scope.ServiceProvider.GetRequiredService<IRichTextSanitizer>();

            var entries = await context.AboutUss.IgnoreTenantFilter().ToListAsync(stoppingToken);
            var cleaned = 0;
            foreach (var tenantEntries in entries.GroupBy(e => e.TenantId))
            {
                using var tenantScope = TenantScope.Begin(tenantEntries.Key);
                var cleanedForTenant = 0;

                foreach (var entry in tenantEntries)
                {
                    var safe = sanitizer.Sanitize(entry.Description);
                    if (safe != entry.Description)
                    {
                        entry.Description = safe;
                        cleanedForTenant++;
                    }
                }

                if (cleanedForTenant > 0)
                {
                    _ = await context.SaveChangesAsync(stoppingToken);
                    cleaned += cleanedForTenant;
                }
            }

            if (cleaned > 0)
            {
                logger.LogInformation("Sanitized {Count} About Us description(s).", cleaned);
            }
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Sanitizing About Us descriptions failed.");
        }
    }
}
