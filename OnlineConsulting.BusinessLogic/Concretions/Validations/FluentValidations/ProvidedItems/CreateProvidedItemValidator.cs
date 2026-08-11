using FluentValidation;
using OnlineConsulting.BusinessLogic.Concretions.Validations.ValidationMessages;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ProvidedItemDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.ProvidedItems;

public class CreateProvidedItemValidator : AbstractValidator<CreateProvidedItemDto>
{
    public CreateProvidedItemValidator()
    {
        RuleFor(x => x.ImgIconId).NotEmpty().WithMessage(ValidationMessage.TheImgIconNotEmpty);

        RuleFor(x => x.Title)
          .NotEmpty().WithMessage(ValidationMessage.TheTitleNotEmpty)
          .MinimumLength(5).WithMessage(ValidationMessage.TheTitleMinimumLength);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ValidationMessage.TheDescriptionNotEmpty)
            .MinimumLength(5).WithMessage(ValidationMessage.TheDescriptionMinimumLength);
    }
}
