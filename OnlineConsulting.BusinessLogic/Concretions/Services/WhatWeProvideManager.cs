using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class WhatWeProvideManager(IMapper mapper, IGenericRepository<WhatWeProvide> repository, IStorageService storageService, IOptions<AppSettingImageFolderPathOption> imageFolderPathOption) : GenericService<WhatWeProvide, IDto>(mapper, repository), IWhatWeProvideService
{
    public async Task<IOperationResult> AddWhatWeProvideAsync(CreateWhatWeProvideDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync(imageFolderPathOption.Value.WhatWeProvideImagesPath, dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the what we provide image", ResultStatus.BadRequest);
    }

    public async Task<IOperationResult> UpdateWhatWeProvideImageAsync(string id, IFormFile image)
    {
        var whatWeProvideResult = await GetByIdAsync<ResultWhatWeProvideDto>(id, false);


        if (!whatWeProvideResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'what we provide' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var whatWeProvide = whatWeProvideResult.Data;

        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        if (!string.IsNullOrEmpty(whatWeProvide.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", whatWeProvide.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(whatWeProvide.ImageUrl);
            }
        }

        var uploadedImageRoute = await storageService.UploadAsync(imageFolderPathOption.Value.WhatWeProvideImagesPath, image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            whatWeProvide.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        var result = await UpdateAsync(whatWeProvide);
        return result.IsSuccessful
            ? new SuccessResult("The what we provide image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the what we provide image. Please try again later.", ResultStatus.InternalServerError);
    }
}
