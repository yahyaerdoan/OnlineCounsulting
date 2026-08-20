using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.Storage.Providers;

namespace OnlineConsulting.Storage;

public static class StorageServiceCollectionExtensions
{
    /// <summary>Registers every backend under its own keyed-DI slot (so a delete can explicitly target the provider a file was actually stored with - see MediaAsset.StorageProvider) plus a plain, unkeyed IStorageService resolved from Storage:ActiveProvider for ordinary upload code that doesn't care which one is active. Same pattern as AddPaymentsInfrastructure.</summary>
    public static IServiceCollection AddStorageInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<StorageOptions>(configuration.GetSection("Storage"));
        var activeProvider = configuration.GetSection("Storage")["ActiveProvider"]
            ?? throw new InvalidOperationException("Storage:ActiveProvider is not configured.");

        _ = services.AddKeyedSingleton<IStorageService, LocalFileSystemStorageService>(StorageProviderNames.Local);
        _ = services.AddKeyedSingleton<IStorageService, S3CompatibleStorageService>(StorageProviderNames.S3);
        _ = services.AddKeyedSingleton<IStorageService, AzureBlobStorageService>(StorageProviderNames.AzureBlob);
        _ = services.AddKeyedSingleton<IStorageService, GoogleCloudStorageService>(StorageProviderNames.GoogleCloud);

        _ = services.AddSingleton(serviceProvider => serviceProvider.GetRequiredKeyedService<IStorageService>(activeProvider));

        return services;
    }
}
