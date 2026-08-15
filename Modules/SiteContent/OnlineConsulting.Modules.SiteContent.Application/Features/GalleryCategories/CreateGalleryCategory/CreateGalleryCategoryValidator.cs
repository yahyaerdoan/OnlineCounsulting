using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.CreateGalleryCategory;

public class CreateGalleryCategoryValidator : AbstractValidator<CreateGalleryCategoryCommand>
{
    public CreateGalleryCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
