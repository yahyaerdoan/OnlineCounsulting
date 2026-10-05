namespace OnlineConsulting.Storage;

/// <summary>Bound from the "Storage:AzureBlob" config section.</summary>
public class AzureBlobStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
