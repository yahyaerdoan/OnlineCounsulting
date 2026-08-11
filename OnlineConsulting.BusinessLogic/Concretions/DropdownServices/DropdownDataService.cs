using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ImgIconDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.DropdownServices;

public class DropdownDataService(IServiceManager serviceManager) : IDropdownDataService
{
    public async Task<List<ResultCategoryDto>> GetCategoriesAsync()
    {
        var result = await serviceManager.CategoryService.GetAllAsync<ResultCategoryDto>(false);

        return result.Data?.ToList() ?? [];
    }
    public async Task<List<ResultClassIconDto>> GetClassIconsAsync()
    {
        var resultIcons = await serviceManager.ClassIconService.GetAllAsync<ResultClassIconDto>(false);

        return resultIcons.Data?.ToList() ?? [];
    }
    public async Task<IQueryable<ResultGalleryCategoryDto>> GetGalleryCategoriesAsync()
    {
        var result = await serviceManager.GalleryCategoryService.GetAllAsync<ResultGalleryCategoryDto>(false);

        return result.Data ?? Enumerable.Empty<ResultGalleryCategoryDto>().AsQueryable();
    }
    public async Task<UpdateGalleryItemWithCategoryDto> PopulateGalleryCategoriesAsync(UpdateGalleryItemWithCategoryDto model)
    {
        var categoriesResult = await serviceManager.GalleryCategoryService.GetAllAsync<ResultGalleryCategoryDto>();
        var allCategories = categoriesResult.Data?.ToList() ?? [];
        model.GalleryCategories ??= [];

        if (model.GalleryCategoryIds is not null)
        {
            foreach (var categoryId in model.GalleryCategoryIds)
            {
                var matchedCategory = allCategories.FirstOrDefault(category => category.Id.ToString() == categoryId);
                if (matchedCategory is not null)
                {
                    model.GalleryCategories.Add(matchedCategory);
                }
            }
        }

        return model;
    }
    public async Task<List<ResultImgIconDto>> GetImgIconsAsync()
    {
        var result = await serviceManager.ImgIconService.GetAllAsync<ResultImgIconDto>(false);

        return result.Data?.ToList() ?? [];
    }

    public async Task<UpdateServiceDto> PopulateServiceGalleryImagesAsync(UpdateServiceDto model)
    {
        var imagesResult = await serviceManager.ServiceImageService.GetAllAsync<ResultServiceImageDto>();
        var allImages = imagesResult.Data?.Where(x => x.ServiceId == model.Id).ToList() ?? [];
        model.ServiceImages ??= [];
        if (model.ServiceImages is not null)
        {
            foreach (var image in allImages)
            {
                model.ServiceImages.Add(image);
            }
        }
        return model;
    }
}
