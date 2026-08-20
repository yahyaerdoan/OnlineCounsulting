using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.CreateUserAddress;

public class CreateUserAddressValidator : AbstractValidator<CreateUserAddressCommand>
{
    public CreateUserAddressValidator()
    {
        _ = RuleFor(x => x.AddressName).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.CompanyName).MaximumLength(200);
        _ = RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(500);
        _ = RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Zipcode).NotEmpty().MaximumLength(20);
        _ = RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
