namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

/// <summary>One US address match while the customer types; State is the two-letter code (TX), the same value the address form stores.</summary>
public sealed record AddressSuggestionResponse(string Formatted, string AddressLine, string City, string State, string Zipcode);
