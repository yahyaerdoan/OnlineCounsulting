using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class GalleryItemManager(IMapper mapper, IGenericRepository<GalleryItem> repository, IStorageService storageService, IGalleryItemRepository galleryItemRepository) : GenericService<GalleryItem, IDto>(mapper, repository), IGalleryItemService
{
    public async Task<IOperationResult> AddGalleryItemAsync(CreateGalleryItemDto dto)
    {
        if (dto.Image is null || dto.Image.Length == 0)
            return new ErrorResult("No image was provided. Please select a valid image file.", ResultStatus.BadRequest);

        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/GalleryItem-Images", dto.Image);
        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            dto.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";

            var galleryItem = _mapper.Map<GalleryItem>(dto);
            galleryItem.GalleryCategories = [.. dto.GalleryCategoryIds
                .Select(id => new GalleryItemCategory
                {
                    GalleryCategoriesId = Guid.Parse(id)
                })];
            await galleryItemRepository.AddGalleryItemAsync(galleryItem);
            await SaveAsync();

            return new SuccessResult("The Gallery Item has been successfully created.", ResultStatus.Created);
        }
        else
            return new ErrorResult($"An error occurred while creating the Gallery Item image", ResultStatus.BadRequest);
    }
    public async Task<IOperationResult<IQueryable<ResultGalleryItemWithCategoriesDto>>> GetAllGalleryItemsAsync<ResultGalleryItemWithCategoriesDto>(bool tracking = true, bool? status = true)
    {
        // Repository üzerinden veriyi sorguluyoruz.
        var galleryItemsQuery = galleryItemRepository.GetAllGalleryItemWithCategories(tracking, status);

        // Verinin varlığını kontrol ediyoruz.

        if (!await galleryItemsQuery.AnyAsync())
            return new ErrorDataResult<IQueryable<ResultGalleryItemWithCategoriesDto>>("No Gallery Item data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<ResultGalleryItemWithCategoriesDto>(galleryItemsQuery);

        return new SuccessDataResult<IQueryable<ResultGalleryItemWithCategoriesDto>>(result, "Gallery Item data retrieved successfully.", ResultStatus.Ok);
    }
    public IOperationResult<UpdateGalleryItemWithCategoryDto> GetByIdGalleryItem<UpdateGalleryItemWithCategoryDto>(string id, bool tracking = true, bool? status = true)
    {
        // Repository üzerinden veriyi sorguluyoruz.
        var galleryItem = galleryItemRepository.GetByIdGalleryItemWithCategories(id, false);

        // Verinin varlığını kontrol ediyoruz.

        if (galleryItem is null)
            return new ErrorDataResult<UpdateGalleryItemWithCategoryDto>("No Gallery Item data found.", ResultStatus.NotFound);

        var result = _mapper.Map<UpdateGalleryItemWithCategoryDto>(galleryItem);

        return new SuccessDataResult<UpdateGalleryItemWithCategoryDto>(result, "Gallery Item data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> RemoveGalleryItemByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var GalleryItemResult = galleryItemRepository.GetByIdGalleryItemWithCategories(id, false);
            if (GalleryItemResult is null)
                return new ErrorResult("GalleryItem not found", ResultStatus.NotFound);
            else
            {
                await storageService.DeleteAsync(GalleryItemResult.ImageUrl);
                _ = galleryItemRepository.RemoveGalleryItemAsync(GalleryItemResult, false);
                await SaveAsync();

                return new SuccessResult("Gallery Item data deleted successfully.", ResultStatus.Ok);
            }

        }
    }
    public async Task<IOperationResult> UpdateGalleryItemAsync(UpdateGalleryItemWithCategoryDto dto)
    {
        var galleryItem = galleryItemRepository.GetByIdGalleryItemWithCategories(dto.Id.ToString());

        if (galleryItem is null)
            return new ErrorResult("Gallery Item not found.", ResultStatus.NotFound);

        // Eski ilişkileri veritabanından temizle
        if (galleryItem.GalleryCategories is not null && galleryItem.GalleryCategories.Count != 0)
        {
            await galleryItemRepository.RemoveGalleryItemsCategory(galleryItem); //geleriye ait kategorileri sil
        }

        // Yeni kategorileri ekle
        galleryItem.GalleryCategories = [.. dto.GalleryCategoryIds
            .Select(id => new GalleryItemCategory
            {
                GalleryCategoriesId = Guid.Parse(id)
            })];

        galleryItem.Description = dto.Description;
        // Güncelleme işlemi
        _repository.Entity.Update(galleryItem);
        await SaveAsync();

        return new SuccessResult("Gallery Item updated successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateGalleryItemImageAsync(string id, IFormFile image)
    {
        var galleryItemResult = await GetByIdAsync<ResultGalleryItemDto>(id, false);


        if (!galleryItemResult.IsSuccessful)
        {
            return new ErrorResult("The requested 'gallery item' information could not be found. Please try again.", ResultStatus.NotFound);
        }

        var galleryItem = galleryItemResult.Data;

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
        if (!string.IsNullOrEmpty(galleryItem.ImageUrl))
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, "wwwroot", galleryItem.ImageUrl.TrimStart('/'));

            if (File.Exists(filePath))
            {
                await storageService.DeleteAsync(galleryItem.ImageUrl);
            }
        }

        // Upload the new image
        var uploadedImageRoute = await storageService.UploadAsync("Resource/LocalStorage/GalleryItem-Images", image);

        if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
        {
            galleryItem.ImageUrl = $"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}";
        }
        else
        {
            return new ErrorResult("The image could not be uploaded. Please try again with a valid file.", ResultStatus.BadRequest);
        }

        // Update entity with new CoverImage
        var result = await UpdateAsync(galleryItem);
        return result.IsSuccessful
            ? new SuccessResult("The cover image has been successfully updated.")
            : new ErrorResult("An unexpected error occurred while updating the gallery item image. Please try again later.", ResultStatus.InternalServerError);
    }
}
