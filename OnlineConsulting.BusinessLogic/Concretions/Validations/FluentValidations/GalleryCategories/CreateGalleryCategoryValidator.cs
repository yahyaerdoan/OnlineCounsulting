using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.GalleryCategories;

internal class CreateGalleryCategoryValidator : AbstractValidator<CreateGalleryCategoryDto>
{
    public CreateGalleryCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessage.TheNameNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheNameMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);
    }
}
