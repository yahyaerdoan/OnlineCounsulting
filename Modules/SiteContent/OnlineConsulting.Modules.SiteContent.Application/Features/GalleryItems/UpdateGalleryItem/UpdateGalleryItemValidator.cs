using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;

public class UpdateGalleryItemValidator : AbstractValidator<UpdateGalleryItemCommand>
{
    public UpdateGalleryItemValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("At least one category must be selected.");
    }
}
