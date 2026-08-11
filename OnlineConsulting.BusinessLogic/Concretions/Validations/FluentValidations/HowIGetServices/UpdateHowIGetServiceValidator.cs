using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.HowIGetServiceDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.HowIGetServices;

internal class UpdateHowIGetServiceValidator : AbstractValidator<UpdateHowIGetServiceDto>
{
    public UpdateHowIGetServiceValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.ImgIconId)
            .NotEmpty().WithMessage("Please choose an icon!");
    }
}
