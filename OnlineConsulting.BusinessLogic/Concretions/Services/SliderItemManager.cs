using AutoMapper;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SliderItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class SliderItemManager(IMapper mapper, IGenericRepository<SliderItem> repository, IStorageService storageService) : GenericService<SliderItem, IDto>(mapper, repository), ISliderItemService
{
    public async Task<IOperationResult> AddSliderItemAsync(CreateSliderItemDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/SliderItem-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the slider item image", ResultStatus.BadRequest);
    }
    public async Task<IOperationResult> RemoveSliderItemByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var SliderItemResult = await GetByIdAsync<ResultSliderItemDto>(id, false);
            if (!SliderItemResult.IsSuccessful)
                return new ErrorResult("Slider item not found", ResultStatus.NotFound);
            else
            {
                var SliderItem = SliderItemResult.Data;
                await storageService.DeleteAsync(SliderItem.ImageUrl);
                var result = await RemoveAsync(SliderItem, false);
                return result;
            }

        }
    }
    public async Task<IOperationResult> UpdateSliderItemImageAsync(string id, IFormFile image)
    {
        var sliderItemResult = await GetByIdAsync<ResultSliderItemDto>(id, false);


        if (!sliderItemResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'slider item' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var sliderItem = sliderItemResult.Data;

        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        if (!string.IsNullOrEmpty(sliderItem.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", sliderItem.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(sliderItem.ImageUrl);
            }
        }

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/SliderItem-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            sliderItem.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        var result = await UpdateAsync(sliderItem);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the slider item image. Please try again later.", ResultStatus.InternalServerError);
    }
}
