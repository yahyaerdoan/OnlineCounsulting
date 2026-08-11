using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.PartnershipSocialMedias;

internal class UpdatePartnershipSocialMediaValidator : AbstractValidator<UpdatePartnershipSocialMediaDto>
{
    public UpdatePartnershipSocialMediaValidator()
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
