namespace OnlineConsulting.Storage;

/// <summary>Bound from the "Storage:GoogleCloud" config section.</summary>
public class GoogleCloudStorageOptions
{
    /// <summary>Raw service-account JSON key content (not a file path) - lets the whole credential live in user-secrets/environment config instead of a file on disk.</summary>
    public string CredentialsJson { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;

    /// <summary>Defaults to Google's own public object URL if left blank - set this only when a CDN fronts the bucket.</summary>
    public string PublicBaseUrl { get; set; } = string.Empty;
}
