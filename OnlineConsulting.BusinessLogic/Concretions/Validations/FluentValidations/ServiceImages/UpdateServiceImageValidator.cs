using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.ServiceImages;

internal class UpdateServiceImageValidator : AbstractValidator<UpdateServiceImageDto>
{
    public UpdateServiceImageValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty().WithMessage("Service cannot be empty!");
    }
}
