using FluentValidation;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

namespace OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.UserAddresses;

internal class CreateUserAddressValidator : AbstractValidator<CreateUserAddressDto>
{
    public CreateUserAddressValidator()
    {
        RuleFor(x => x.AddressLine).NotEmpty().WithMessage("Address Line Must be not empty !");
        RuleFor(x => x.City).NotEmpty().WithMessage("City Must be not empty !");
        RuleFor(x => x.Country).NotEmpty().WithMessage("Country Must be not empty !");
        RuleFor(x => x.Zipcode).NotEmpty().WithMessage("Must be not empty !");
        RuleFor(x => x.AddressName).NotEmpty().WithMessage("Address name must be not empty !");
    }
}
