using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.ClassIcons;

internal class CreateClassIconValidator : AbstractValidator<CreateClassIconDto>
{
    public CreateClassIconValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessage.TheNameNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheNameMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.IconClass)
            .NotEmpty().WithMessage("Icon class cannot be empty!");
    }
}
