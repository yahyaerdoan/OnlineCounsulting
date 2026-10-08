namespace OnlineConsulting.Storage;

/// <summary>Covers any S3-compatible backend (AWS S3, Cloudflare R2, Backblaze B2) - ServiceUrl is what actually picks the backend.</summary>
public class S3StorageOptions
{
    public string ServiceUrl { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;

    /// <summary>Real AWS S3 requires a region; R2/B2 accept "auto" or are region-less.</summary>
    public string Region { get; set; } = "auto";

    /// <summary>Public URL prefix used to build the returned Url - often a separate CDN/public domain from ServiceUrl (which is the API endpoint, not necessarily public-facing).</summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
