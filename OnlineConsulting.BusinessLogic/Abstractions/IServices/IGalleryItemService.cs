using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IGalleryItemService : IGenericService<GalleryItem, IDto>
{
    Task<IOperationResult<IQueryable<ResultGalleryItemWithCategoriesDto>>> GetAllGalleryItemsAsync<ResultGalleryItemWithCategoriesDto>(bool tracking = true, bool? status = true);
    IOperationResult<ResultGalleryItemDto> GetByIdGalleryItem<ResultGalleryItemDto>(string id, bool traking = true, bool? status = true);
    Task<IOperationResult> AddGalleryItemAsync(CreateGalleryItemDto dto);
    Task<IOperationResult> UpdateGalleryItemAsync(UpdateGalleryItemWithCategoryDto dto);
    Task<IOperationResult> UpdateGalleryItemImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemoveGalleryItemByIdAsync(string id, bool isSoftDelete = true);
}
