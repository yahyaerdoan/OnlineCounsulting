namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/addresses/suggestions - one US address match while typing; State is the two-letter code.</summary>
public record AddressSuggestionResponse(string Formatted, string AddressLine, string City, string State, string Zipcode);
