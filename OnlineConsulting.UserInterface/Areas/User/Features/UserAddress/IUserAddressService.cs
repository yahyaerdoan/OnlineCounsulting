using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>CRUD + billing/shipping selection for the current user's addresses via /api/addresses.</summary>
public interface IUserAddressService
{
    Task<List<UserAddressResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserAddressResponse?> GetBillingAsync(CancellationToken cancellationToken = default);
    Task<UserAddressResponse?> GetShippingAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope<Guid>> CreateAsync(string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(Guid id, string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> SetBillingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> SetShippingAsync(Guid id, CancellationToken cancellationToken = default);
}

public record UserAddressResponse(Guid Id, string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress);
