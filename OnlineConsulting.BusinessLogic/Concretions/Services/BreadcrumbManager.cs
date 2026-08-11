using AutoMapper;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BreadcrumbDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class BreadcrumbManager(IMapper mapper, IGenericRepository<Breadcrumb> repository, IStorageService storageService) : GenericService<Breadcrumb, IDto>(mapper, repository), IBreadcrumbService
{
    public async Task<IOperationResult> AddBreadcrumbAsync(CreateBreadcrumbDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Breadcrumb-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the Breadcrumb image", ResultStatus.BadRequest);
    }
    public async Task<IOperationResult> RemoveBreadcrumbByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var BreadcrumbResult = await GetByIdAsync<ResultBreadcrumbDto>(id, false);
            if (!BreadcrumbResult.IsSuccessful)
                return new ErrorResult("Breadcrumb not found", ResultStatus.NotFound);
            else
            {
                var Breadcrumb = BreadcrumbResult.Data;
                await storageService.DeleteAsync(Breadcrumb.ImageUrl);
                var result = await RemoveAsync(Breadcrumb, false);
                return result;
            }

        }
    }
    public async Task<IOperationResult> UpdateBreadcrumbImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image)
    {
        var breadcrumbResult = await GetByIdAsync<ResultBreadcrumbDto>(id, false);


        if (!breadcrumbResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'Breadcrumb' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var breadcrumb = breadcrumbResult.Data;

        // Validate image before processing
        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the breadcrumb photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        // Check MIME type validity
        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        // Delete the existing image only if a valid new image is provided
        if (!string.IsNullOrEmpty(breadcrumb.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", breadcrumb.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(breadcrumb.ImageUrl);
            }
        }

        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Breadcrumb-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            breadcrumb.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        // Update entity with new CoverImage
        var result = await UpdateAsync(breadcrumb);
        return result.IsSuccessful
            ? new SuccessResult("The image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the breadcrumb image. Please try again later.", ResultStatus.InternalServerError);

    }
}
