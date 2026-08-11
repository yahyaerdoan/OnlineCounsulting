using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.WhatWeProvide;

public class UpdateWhatWeProvideValidator : AbstractValidator<UpdateWhatWeProvideDto>
{
    public UpdateWhatWeProvideValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);
    }
}
