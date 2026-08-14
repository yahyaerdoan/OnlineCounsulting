using AutoMapper;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class FooterAboutManager(IMapper mapper, IGenericRepository<FooterAbout> repository, IStorageService storageService) : GenericService<FooterAbout, IDto>(mapper, repository), IFooterAboutService
{
    public async Task<IOperationResult> AddFooterAboutAsync(CreateFooterAboutDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/FooterAbout-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the Footer About image", ResultStatus.BadRequest);
    }
    public async Task<IOperationResult> RemoveFooterAboutByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var FooterAboutResult = await GetByIdAsync<ResultFooterAboutDto>(id, false);
            if (!FooterAboutResult.IsSuccessful)
                return new ErrorResult("FooterAbout not found", ResultStatus.NotFound);
            else
            {
                var FooterAbout = FooterAboutResult.Data;
                await storageService.DeleteAsync(FooterAbout.ImageUrl);
                var result = await RemoveAsync(FooterAbout, false);
                return result;
            }

        }
    }
    public async Task<IOperationResult> UpdateFooterAboutImageAsync(string id, IFormFile image)
    {
        var footerAboutResult = await GetByIdAsync<ResultFooterAboutDto>(id, false);


        if (!footerAboutResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'Footer About' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var footerAbout = footerAboutResult.Data;

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
        if (!string.IsNullOrEmpty(footerAbout.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", footerAbout.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(footerAbout.ImageUrl);
            }
        }

        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/FooterAbout-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            footerAbout.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        // Update entity with new CoverImage
        var result = await UpdateAsync(footerAbout);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the footer about image. Please try again later.", ResultStatus.InternalServerError);
    }
}
