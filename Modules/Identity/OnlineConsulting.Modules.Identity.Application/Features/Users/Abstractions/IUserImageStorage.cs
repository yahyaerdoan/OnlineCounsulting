
namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;

public interface IUserImageStorage
{
    /// <summary>Stores the image under a new name (keeping the file extension) and returns its public URL.</summary>
    Task<string> UploadAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default);
}
