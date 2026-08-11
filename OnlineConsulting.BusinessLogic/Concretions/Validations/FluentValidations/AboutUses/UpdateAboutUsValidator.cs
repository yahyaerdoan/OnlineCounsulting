using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.AboutUses;

internal class UpdateAboutUsValidator : AbstractValidator<UpdateAboutUsDto>
{
    public UpdateAboutUsValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
             .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);

        RuleFor(x => x.VideoUrl)
           .NotEmpty().WithMessage(ValidationMessage.TheUrlNotEmpty)
           .MinimumLength(17).WithMessage(ValidationMessage.TheUrlMinimumLength)
           .Matches(ValidationMessage.TheUrlMatches)
           .WithMessage(ValidationMessage.TheUrlMatchesExample);
    }
}
