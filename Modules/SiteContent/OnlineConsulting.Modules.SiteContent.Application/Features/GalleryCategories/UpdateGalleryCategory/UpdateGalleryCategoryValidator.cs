using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.UpdateGalleryCategory;

public class UpdateGalleryCategoryValidator : AbstractValidator<UpdateGalleryCategoryCommand>
{
    public UpdateGalleryCategoryValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Description).MaximumLength(500);
    }
}
