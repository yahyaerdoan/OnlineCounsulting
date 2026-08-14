using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class AboutUsManager(IMapper mapper, IGenericRepository<AboutUs> repository, IStorageService storageService, IOptions<AppSettingImageFolderPathOption> imageFolderPathOption)
    : GenericService<AboutUs, IDto>(mapper, repository), IAboutUsService
{
    public async Task<IOperationResult> AddAboutUsAsync(CreateAboutUsDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync(imageFolderPathOption.Value.AboutUsImagesPath, dto.Image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.CoverImage = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }

        return new ErrorResult("The cover image could not be uploaded. Please make sure the image meets the required format (JPEG, JPG, PNG, GIF).", ResultStatus.BadRequest);
    }

    public async Task<IOperationResult> UpdateAboutUsImageAsync(string id, IFormFile image)
    {
        var aboutUsResult = await GetByIdAsync<ResultAboutUsDto>(id, false);

        if (!aboutUsResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'About Us' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var aboutUs = aboutUsResult.Data;

        // Validate image before processing
        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the cover photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        // Check MIME type validity
        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        // Delete the existing image only if a valid new image is provided
        if (!string.IsNullOrEmpty(aboutUs.CoverImage))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", aboutUs.CoverImage.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(aboutUs.CoverImage);
            }
        }

        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync(imageFolderPathOption.Value.AboutUsImagesPath, image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            aboutUs.CoverImage = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        // Update entity with new CoverImage
        var result = await UpdateAsync(aboutUs);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the cover image. Please try again later.", ResultStatus.InternalServerError);
    }
}
