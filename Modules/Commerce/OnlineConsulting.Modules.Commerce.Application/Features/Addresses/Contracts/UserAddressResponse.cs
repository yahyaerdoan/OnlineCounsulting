using Hateoas;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

public class UserAddressResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string AddressName { get; init; }
    public string? CompanyName { get; init; }
    public required string Country { get; init; }
    public required string AddressLine { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string Zipcode { get; init; }
    public string? Notes { get; init; }
    public required bool IsShippingAddress { get; init; }
    public required bool IsBillingAddress { get; init; }

    public static UserAddressResponse FromDomain(UserAddress address) => new()
    {
        Id = address.Id,
        AddressName = address.AddressName,
        CompanyName = address.CompanyName,
        Country = address.Country,
        AddressLine = address.AddressLine,
        City = address.City,
        State = address.State,
        Zipcode = address.Zipcode,
        Notes = address.Notes,
        IsShippingAddress = address.IsShippingAddress,
        IsBillingAddress = address.IsBillingAddress,
    };
}
