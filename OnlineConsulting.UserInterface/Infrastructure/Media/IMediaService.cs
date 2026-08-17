namespace OnlineConsulting.UserInterface.Infrastructure.Media;

/// <summary>Upload/resolve for the Media module's MediaAsset, shared by every feature service that needs to turn
/// a form-uploaded photo into a MediaAssetId (Partnership today; GalleryItem/Service/etc. as they migrate) or
/// display an already-stored one - kept out of any single controller/service so this logic exists exactly once.</summary>
public interface IMediaService
{
    /// <summary>Null input (no file selected) returns null - the caller decides what "no new photo" means for its own update flow.</summary>
    Task<Guid?> UploadAsync(IFormFile? file, CancellationToken cancellationToken = default);

    Task<string?> ResolveUrlAsync(Guid? mediaAssetId, CancellationToken cancellationToken = default);
}
