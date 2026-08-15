using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Media.Domain;

/// <summary>A real uploaded file (photo, not an icon - icons are a plain MudBlazor value string directly on whichever entity displays one). Uploaded once, referenced by plain id (MediaAssetId) from any module that needs to show an image - Service cover photo, GalleryItem, HeroSlide, etc.</summary>
public class MediaAsset : TenantEntity<Guid>
{
    public required string Url { get; set; }
    public string? AltText { get; set; }
    public required string ContentType { get; set; }
    public required long SizeBytes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    /// <summary>Which IStorageService uploaded this - a delete must route back through the same provider that stored it, not whichever is active later.</summary>
    public required string StorageProvider { get; set; }

    /// <summary>Free-form JSON for extras that don't need their own column (focal point, blurhash placeholder, source/license info) - same rationale as SiteContent's Metadata.</summary>
    public string? Metadata { get; set; }
}
