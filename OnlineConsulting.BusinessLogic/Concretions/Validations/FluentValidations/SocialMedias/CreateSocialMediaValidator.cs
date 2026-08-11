using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SocialMediaDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.SocialMedias;

internal class CreateSocialMediaValidator : AbstractValidator<CreateSocialMediaDto>
{
    public CreateSocialMediaValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessage.TheNameNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheNameMinimumLength);

        RuleFor(x => x.Url)
           .NotEmpty().WithMessage(ValidationMessage.TheUrlNotEmpty)
           .MinimumLength(17).WithMessage(ValidationMessage.TheUrlMinimumLength)
           .Matches(ValidationMessage.TheUrlMatches)
           .WithMessage(ValidationMessage.TheUrlMatchesExample);

        RuleFor(x => x.ClassIconId)
            .NotEmpty().WithMessage(ValidationMessage.TheClassIconNotEmpty);
    }
}
