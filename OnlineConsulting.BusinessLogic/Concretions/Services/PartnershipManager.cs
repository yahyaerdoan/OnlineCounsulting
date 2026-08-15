using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class PartnershipManager(IMapper mapper, IGenericRepository<Partnership> repository, IStorageService storageService, IPartnershipRepository partnershipRepository) : GenericService<Partnership, IDto>(mapper, repository), IPartnershipService
{
    public async Task<IOperationResult> AddPartnershipAsync(CreatePartnershipDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Partnership-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var result = await AddAsync(dto);
            return result;
        }
        else
            return new ErrorResult($"An error occurred while creating the partnership image", ResultStatus.BadRequest);
    }

    public async Task<IOperationResult<IQueryable<TDto>>> GetAllPartnershipsWithSocialMediasAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var entities = partnershipRepository.GetAllPartnershipsWithSocialMedias(tracking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(Partnership).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }

    public async Task<IOperationResult> RemovePartnershipByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var partnershipResult = await GetByIdAsync<ResultPartnershipDto>(id, false);
            if (!partnershipResult.IsSuccessful)
                return new ErrorResult("Partnership not found", ResultStatus.NotFound);
            else
            {
                var partnership = partnershipResult.Data;
                await storageService.DeleteAsync(partnership.ImageUrl);
                var result = await RemoveAsync(partnership, false);
                return result;
            }

        }
    }
    public async Task<IOperationResult> UpdatePartnershipImageAsync(string id, IFormFile image)
    {
        var partnershipResult = await GetByIdAsync<ResultPartnershipDto>(id, false);


        if (!partnershipResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'partnership' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var partnership = partnershipResult.Data;

        if (image is null || image.Length == 0)
        {
            return new ErrorResult("No image was provided. Please select a valid image file to update the photo.", ResultStatus.BadRequest);
        }

        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(image.ContentType))
        {
            return new ErrorResult("Invalid image format. Please upload an image in JPEG, JPG, PNG, or GIF format.", ResultStatus.BadRequest);
        }

        if (!string.IsNullOrEmpty(partnership.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", partnership.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(partnership.ImageUrl);
            }
        }

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/Partnership-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            partnership.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        var result = await UpdateAsync(partnership);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the partnership image. Please try again later.", ResultStatus.InternalServerError);
    }
}
