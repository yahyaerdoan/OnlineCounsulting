using Core.PersistenceLayer.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Media.Infrastructure.Persistence;
using OnlineConsulting.SharedKernel.Media;

namespace OnlineConsulting.Modules.Media.Infrastructure.PublicUrls;

/// <summary>Reads by id across tenants, since the caller (email dispatch, PDF rendering) may run outside the asset owner's request.</summary>
public class MediaAssetUrlReader(MediaDbContext context, IOptions<MediaPublicUrlOptions> options) : IMediaAssetUrlReader
{
    public async Task<string?> GetPublicUrlAsync(Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var url = await context.MediaAssets
            .IgnoreTenantFilter()
            .Where(a => a.Id == mediaAssetId)
            .Select(a => a.Url)
            .FirstOrDefaultAsync(cancellationToken);

        return url is not null && url.StartsWith('/') ? $"{options.Value.PublicOrigin.TrimEnd('/')}{url}" : url;
    }
}
