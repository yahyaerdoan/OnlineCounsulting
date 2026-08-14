using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;

public record UpdateUserImageCommand(Guid UserId, IFormFile Image) : IRequest<OperationResult>;

public class UpdateUserImageHandler(UserManager<User> userManager, IUserImageStorage imageStorage)
    : IRequestHandler<UpdateUserImageCommand, OperationResult>
{
    private const string DefaultImageUrl = "/Resource/LocalStorage/DefaultImages/defaultUserImage.png";
    private static readonly HashSet<string> AllowedMimeTypes = ["image/jpeg", "image/jpg", "image/png", "image/gif"];

    public async Task<OperationResult> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.NotFound("The requested 'user' information could not be found. Please try again.");

        if (request.Image is null || request.Image.Length == 0)
            return Result.BadRequest("No image was provided. Please select a valid image file to update the photo.");

        if (!AllowedMimeTypes.Contains(request.Image.ContentType))
            return Result.BadRequest("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.");

        if (!string.IsNullOrEmpty(user.ImageUrl) && user.ImageUrl != DefaultImageUrl)
            await imageStorage.DeleteAsync(user.ImageUrl, cancellationToken);

        user.ImageUrl = await imageStorage.UploadAsync(request.Image, cancellationToken);

        var result = await userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success("The user profile image has been successfully updated.")
            : Result.InternalServerError("An unexpected error occurred while updating the user profile image. Please try again later.");
    }
}
