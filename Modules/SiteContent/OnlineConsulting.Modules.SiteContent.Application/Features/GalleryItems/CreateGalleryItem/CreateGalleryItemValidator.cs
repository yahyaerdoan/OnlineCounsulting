using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.CreateGalleryItem;

public class CreateGalleryItemValidator : AbstractValidator<CreateGalleryItemCommand>
{
    public CreateGalleryItemValidator()
    {
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.CategoryIds).NotEmpty().WithMessage("At least one category must be selected.");
    }
}
