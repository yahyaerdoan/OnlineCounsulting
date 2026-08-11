using AutoMapper;
using Microsoft.AspNetCore.Http;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;


namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class ServiceImageManager(IMapper mapper, IGenericRepository<ServiceImage> repository, IStorageService storageService) : GenericService<ServiceImage, IDto>(mapper, repository), IServiceImageService
{
    public async Task<IOperationResult> AddServiceImageAsync(CreateServiceImageDto dto)
    {
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Service-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the cover image", ResultStatus.BadRequest);
    }

    public async Task<IOperationResult> RemoveServiceImageByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var serviceImageResult = await GetByIdAsync<ResultServiceImageDto>(id, false);
            if (!serviceImageResult.IsSuccessful)
                return new ErrorResult("ServiceImage not found", ResultStatus.NotFound);
            else
            {
                var serviceImage = serviceImageResult.Data;
                await storageService.DeleteAsync(serviceImage.ImageUrl);
                var result = await RemoveAsync(serviceImage, false);
                return result;
            }
        }
    }

    public async Task<IOperationResult> UpdateServiceImageAsync(string id, IFormFile image)
    {
        var serviceImageResult = await GetByIdAsync<ResultServiceImageDto>(id, false);


        if (!serviceImageResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'service image' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var serviceImage = serviceImageResult.Data;

        // Validate image before processing
        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        // Check MIME type validity
        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        // Delete the existing image only if a valid new image is provided
        if (!string.IsNullOrEmpty(serviceImage.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", serviceImage.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(serviceImage.ImageUrl);
            }
        }

        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Service-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            serviceImage.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        // Update entity with new CoverImage
        var result = await UpdateAsync(serviceImage);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the service image. Please try again later.", ResultStatus.InternalServerError);
    }
}
