namespace OnlineConsulting.UserInterface.Infrastructure.Media;

/// <summary>Upload/resolve for the Media module's MediaAsset, shared by every feature service that turns a form-uploaded photo into a MediaAssetId.</summary>
public interface IMediaService
{
    /// <summary>Null input (no file selected) returns null - the caller decides what "no new photo" means for its own update flow.</summary>
    Task<Guid?> UploadAsync(IFormFile? file, CancellationToken cancellationToken = default);

    Task<string?> ResolveUrlAsync(Guid? mediaAssetId, CancellationToken cancellationToken = default);
}
