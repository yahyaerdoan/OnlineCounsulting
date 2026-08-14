using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages.ILocalStorages;
using OnlineConsulting.BusinessLogic.Concretions.StorageServices.Utilities.FileHelpers;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.StorageServices.Storages.LocalStorages;


public class LocalStorage(IWebHostEnvironment webHostEnvironment) : FileNameHelper, ILocalStorage
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async Task DeleteAsync(string imageUrl)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var filePath = $"{rootPath.Replace("\\", "/")}/wwwroot{imageUrl}";
        if (File.Exists(filePath))
            File.Delete(filePath);
        await Task.CompletedTask;
    }

    public Task<List<string>> GetFiles(string targetFolderPathOrContainerName)
    {
        DirectoryInfo directoryInfo = new(targetFolderPathOrContainerName);
        List<string> fileNames = [.. directoryInfo.GetFiles().Select(x => x.Name)];
        return Task.FromResult(fileNames);
    }

    public bool HasFile(string targetFolderPathOrContainerName, string fileName)
    {
        var filePath = $"{targetFolderPathOrContainerName}\\{fileName}";
        return File.Exists(filePath);
    }

    public async Task<IOperationResult<(string FileName, string FileExtension, string FullPath, string TargetFolderPathOrContainerName)>> UploadAsync(string targetFolderPathOrContainerName, IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return new ErrorDataResult<(string, string, string, string)>("No file uploaded.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        if (!allowedMimeTypes.Contains(file.ContentType))
        {
            return new ErrorDataResult<(string, string, string, string)>("Invalid file format. Only .jpeg, .jpg, .png, or .gif files are allowed.", ResultStatus.BadRequest);
        }

        var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, targetFolderPathOrContainerName);
        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        if (string.IsNullOrEmpty(file.FileName))
        {
            return new ErrorDataResult<(string, string, string, string)>("File name is not valid.", ResultStatus.BadRequest);
        }

        var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
        var fileExtension = Path.GetExtension(file.FileName);

        var newFileName = await GenerateUniqueFileNameAsync(originalFileName, fileExtension, uploadDirectory, HasFile);
        var fullFilePath = Path.Combine(uploadDirectory, newFileName);

        var isFileCopied = await CopyFileAsync(fullFilePath, file);
        if (!isFileCopied)
        {
            return new ErrorDataResult<(string, string, string, string)>("File could not be saved.", ResultStatus.BadRequest);
        }

        return new SuccessDataResult<(string, string, string, string)>((newFileName, fileExtension, fullFilePath, targetFolderPathOrContainerName),
            "File uploaded successfully.", ResultStatus.Created);
    }

    #region Private Helper Methods, Only Using Here
    private static async Task<bool> CopyFileAsync(string destinationFilePath, IFormFile sourceFile)
    {
        await using var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: false);
        await sourceFile.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
        return true;
    }
    #endregion
}
