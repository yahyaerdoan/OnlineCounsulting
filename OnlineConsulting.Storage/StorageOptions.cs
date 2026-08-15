namespace OnlineConsulting.Storage;

/// <summary>Bound from the "Storage" config section. ActiveProvider is the one lever that switches backends - everything else stays wired regardless of which one is active.</summary>
public class StorageOptions
{
    public required string ActiveProvider { get; set; }
    public LocalStorageOptions Local { get; set; } = new();
    public S3StorageOptions S3 { get; set; } = new();
    public AzureBlobStorageOptions AzureBlob { get; set; } = new();
    public GoogleCloudStorageOptions GoogleCloud { get; set; } = new();
}

public class LocalStorageOptions
{
    /// <summary>Absolute path files are written to - defaults to the API host's own wwwroot/media so UseStaticFiles() serves them directly.</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>URL prefix returned to callers, e.g. "/media" - must match the static files mapping for RootPath.</summary>
    public string PublicPathPrefix { get; set; } = "/media";
}

/// <summary>Covers any S3-compatible backend - real AWS S3, Cloudflare R2, or Backblaze B2 - through one implementation, since they all speak the same API. ServiceUrl is what actually picks the backend (blank/AWS's own endpoint for real S3, the account-specific endpoint for R2/B2).</summary>
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

public class AzureBlobStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}

public class GoogleCloudStorageOptions
{
    /// <summary>Raw service-account JSON key content (not a file path) - lets the whole credential live in user-secrets/environment config instead of a file on disk.</summary>
    public string CredentialsJson { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;

    /// <summary>Defaults to Google's own public object URL if left blank - set this only when a CDN fronts the bucket.</summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
