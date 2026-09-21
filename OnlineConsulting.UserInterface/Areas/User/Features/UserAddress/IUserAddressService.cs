using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>CRUD + billing/shipping selection for the current user's addresses via /api/addresses.</summary>
public interface IUserAddressService
{
    /// <summary>Gets all addresses for the current user.</summary>
    Task<List<UserAddressResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the current user's billing address, if set.</summary>
    Task<UserAddressResponse?> GetBillingAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the current user's shipping address, if set.</summary>
    Task<UserAddressResponse?> GetShippingAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new address for the current user.</summary>
    /// <returns>Envelope carrying the new address id.</returns>
    Task<ApiEnvelope<Guid>> CreateAsync(string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing address.</summary>
    Task<ApiEnvelope> UpdateAsync(Guid id, string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default);

    /// <summary>Deletes an address.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Marks an address as the current user's billing address.</summary>
    Task<ApiEnvelope> SetBillingAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Marks an address as the current user's shipping address.</summary>
    Task<ApiEnvelope> SetShippingAsync(Guid id, CancellationToken cancellationToken = default);
}

public record UserAddressResponse(Guid Id, string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress);
