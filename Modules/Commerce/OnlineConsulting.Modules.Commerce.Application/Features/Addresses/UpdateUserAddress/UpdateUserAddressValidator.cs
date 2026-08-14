using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;

public class UpdateUserAddressValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AddressName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CompanyName).MaximumLength(200);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.AddressLine).NotEmpty().MaximumLength(500);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Zipcode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
