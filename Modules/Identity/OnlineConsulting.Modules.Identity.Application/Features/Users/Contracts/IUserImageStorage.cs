using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;

public interface IUserImageStorage
{
    Task<string> UploadAsync(IFormFile image, CancellationToken cancellationToken = default);
    Task DeleteAsync(string imageUrl, CancellationToken cancellationToken = default);
}
