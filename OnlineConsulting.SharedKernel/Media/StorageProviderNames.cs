namespace OnlineConsulting.SharedKernel.Media;

/// <summary>Values for Storage:ActiveProvider, also the keyed-DI keys of IStorageService.</summary>
public static class StorageProviderNames
{
    public const string Local = "Local";
    public const string AzureBlob = "AzureBlob";
    public const string S3 = "S3";
    public const string GoogleCloud = "GoogleCloud";
}
