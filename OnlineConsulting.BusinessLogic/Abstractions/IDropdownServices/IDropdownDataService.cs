using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ImgIconDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;

public interface IDropdownDataService
{
    Task<List<ResultImgIconDto>> GetImgIconsAsync();
    Task<List<ResultClassIconDto>> GetClassIconsAsync();
    Task<List<ResultCategoryDto>> GetCategoriesAsync();
    Task<IQueryable<ResultGalleryCategoryDto>> GetGalleryCategoriesAsync();
    Task<UpdateGalleryItemWithCategoryDto> PopulateGalleryCategoriesAsync(UpdateGalleryItemWithCategoryDto model);
    Task<UpdateServiceDto> PopulateServiceGalleryImagesAsync(UpdateServiceDto model);
}
