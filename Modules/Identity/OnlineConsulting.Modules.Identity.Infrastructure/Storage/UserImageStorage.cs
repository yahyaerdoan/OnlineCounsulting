using Microsoft.AspNetCore.Hosting;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Storage;

public class UserImageStorage(IWebHostEnvironment environment) : IUserImageStorage
{
    private const string TargetFolder = "Resource/LocalStorage/User-Images";

    public async Task<string> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var storedName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var folderPath = Path.Combine(environment.WebRootPath, TargetFolder);
        _ = Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, storedName);
        await using (var stream = File.Create(filePath))
        {
            await content.CopyToAsync(stream, cancellationToken);
        }

        return $"/{TargetFolder}/{storedName}";
    }

    public Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(environment.WebRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
