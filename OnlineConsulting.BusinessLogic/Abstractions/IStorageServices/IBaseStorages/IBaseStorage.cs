using ResultHandler.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IBaseStorages;

public interface IBaseStorage
{
    Task<IOperationResult<(string FileName, string FileExtension, string FullPath, string TargetFolderPathOrContainerName)>> UploadAsync(string targetFolderPathOrContainerName, IFormFile files);
    Task DeleteAsync(string imageUrl);
    Task<List<string>> GetFiles(string targetFolderPathOrContainerName);
    bool HasFile(string targetFolderPathOrContainerName, string fileName);
}
