using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;

public class ValidationFilter(IDropdownDataService dropdownDataService) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState is not null && !context.ModelState.IsValid)
        {
            if (context.Controller is Controller controller)
            {
                var model = context.ActionArguments.Values.FirstOrDefault();

                controller.ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();
                controller.ViewBag.GetClassIcons = await dropdownDataService.GetClassIconsAsync();
                controller.ViewBag.GetCategories = await dropdownDataService.GetCategoriesAsync();
                controller.ViewBag.GetGalleryCategories = await dropdownDataService.GetGalleryCategoriesAsync();

                if (model is CreatePartnershipSocialMediaDto partnershipSocialmediaDto)
                    controller.ViewBag.PartnershipId = partnershipSocialmediaDto.PartnershipId;
                if (model is UpdatePartnershipSocialMediaDto partnershipSocialMediaDto)
                    controller.ViewBag.PartnershipId = partnershipSocialMediaDto.PartnershipId;

                context.Result = controller.View(model is null ? null : await TransformModelAsync(model));
                return;
            }

            var errors = context.ModelState
                .Where(x => x.Value is not null && x.Value.Errors.Any())
                .ToDictionary(
                    e => e.Key,
                    e => e.Value?.Errors.Select(error => error.ErrorMessage).ToArray() ?? []
                );

            context.Result = new BadRequestObjectResult(errors);
            return;
        }
        await next();
    }
    private async Task<object> TransformModelAsync(object model)
    {
        if (model is UpdateGalleryItemWithCategoryDto galleryItemModel)
        {
            return await dropdownDataService.PopulateGalleryCategoriesAsync(galleryItemModel);
        }
        else if (model is UpdateServiceDto serviceWithImageModel)
        {
            return await dropdownDataService.PopulateServiceGalleryImagesAsync(serviceWithImageModel);
        }

        return model;
    }
}
