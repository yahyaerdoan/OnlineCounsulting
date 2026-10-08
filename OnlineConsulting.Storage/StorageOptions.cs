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
