using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IBaseStorages;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Concretions.StorageServices.Storages;

public class StorageService(IBaseStorage storage) : IStorageService
{
    private readonly IBaseStorage _storage = storage;

    public string StorageName => _storage.GetType().Name;

    public async Task DeleteAsync(string imageUrl)
        => await _storage.DeleteAsync(imageUrl);
    public async Task<List<string>> GetFiles(string targetFolderPathOrContainerName)
        => await _storage.GetFiles(targetFolderPathOrContainerName);

    public bool HasFile(string targetFolderPathOrContainerName, string fileName)
        => _storage.HasFile(targetFolderPathOrContainerName, fileName);

    public Task<IOperationResult<(string FileName, string FileExtension, string FullPath, string TargetFolderPathOrContainerName)>> UploadAsync(string targetFolderPathOrContainerName, IFormFile file)
         => _storage.UploadAsync(targetFolderPathOrContainerName, file);
}
