namespace OnlineConsulting.SharedKernel.Media;

/// <summary>Cross-module read of a media asset's address; implemented by the Media module.</summary>
public interface IMediaAssetUrlReader
{
    /// <summary>An absolute URL that works outside the site (emails, PDFs), or null when the asset doesn't exist.</summary>
    Task<string?> GetPublicUrlAsync(Guid mediaAssetId, CancellationToken cancellationToken = default);
}
