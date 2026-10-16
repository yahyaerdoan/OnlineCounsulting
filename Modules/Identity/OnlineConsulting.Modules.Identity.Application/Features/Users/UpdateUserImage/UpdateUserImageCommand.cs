using MediatR;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;

/// <summary>The image as plain stream and metadata; the Api turns whatever transport it received into these, keeping the command transport-agnostic.</summary>
public record UpdateUserImageCommand(Guid UserId, Stream Content, string FileName, string ContentType, long Length) : IRequest<OperationResult>;

/// <summary>Stores the photo in media storage under the user's tenant folder.</summary>
public class UpdateUserImageHandler(UserManager<User> userManager, IStorageService storageService)
    : IRequestHandler<UpdateUserImageCommand, OperationResult>
{
    private const string Folder = "profile-images";
    private static readonly HashSet<string> AllowedMimeTypes = ["image/jpeg", "image/jpg", "image/png", "image/gif"];

    public async Task<OperationResult> Handle(UpdateUserImageCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            return Result.NotFound("The requested 'user' information could not be found. Please try again.");
        }

        if (request.Length == 0)
        {
            return Result.UnprocessableContent("No image was provided. Please select a valid image file to update the photo.");
        }

        if (!AllowedMimeTypes.Contains(request.ContentType))
        {
            return Result.UnprocessableContent("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.");
        }

        var previousUrl = user.ImageUrl;
        var upload = await storageService.UploadAsync(request.Content, request.FileName, request.ContentType, TenantMediaFolder.For(user.TenantId, Folder), cancellationToken);
        user.ImageUrl = upload.Url;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            await storageService.DeleteAsync(upload.Url, cancellationToken);
            return Result.InternalServerError("An unexpected error occurred while updating the user profile image. Please try again later.");
        }

        // Old photo goes only after the new one is saved.
        if (!string.IsNullOrEmpty(previousUrl))
        {
            await storageService.DeleteAsync(previousUrl, cancellationToken);
        }

        return Result.Success("The user profile image has been successfully updated.");
    }
}
