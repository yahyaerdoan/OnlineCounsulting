namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/addresses's response shape - current-user-scoped saved addresses.</summary>
public record UserAddressResponse(
    Guid Id,
    string AddressName,
    string? CompanyName,
    string Country,
    string AddressLine,
    string City,
    string State,
    string Zipcode,
    string? Notes,
    bool IsShippingAddress,
    bool IsBillingAddress);
