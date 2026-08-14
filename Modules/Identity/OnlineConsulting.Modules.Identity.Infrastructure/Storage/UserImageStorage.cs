using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Storage;

public class UserImageStorage(IWebHostEnvironment environment) : IUserImageStorage
{
    private const string TargetFolder = "Resource/LocalStorage/User-Images";

    public async Task<string> UploadAsync(IFormFile image, CancellationToken cancellationToken = default)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var folderPath = Path.Combine(environment.WebRootPath, TargetFolder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await using (var stream = File.Create(filePath))
            await image.CopyToAsync(stream, cancellationToken);

        return $"/{TargetFolder}/{fileName}";
    }

    public Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(environment.WebRootPath, imageUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
