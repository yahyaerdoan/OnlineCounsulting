using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Formats a saved address as a single display line - shared by Checkout's address
/// cards and the address-selection dialog so they read identically.</summary>
public static class AddressFormat
{
    public static string FullLine(UserAddressResponse address) =>
        $"{address.AddressLine}, {address.City}, {address.State} {address.Zipcode}, {address.Country}";
}
