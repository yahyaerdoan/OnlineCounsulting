using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.Categories;

internal class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.ImgIconId)
            .NotEmpty().WithMessage(ValidationMessage.TheImgIconNotEmpty);
    }
}
