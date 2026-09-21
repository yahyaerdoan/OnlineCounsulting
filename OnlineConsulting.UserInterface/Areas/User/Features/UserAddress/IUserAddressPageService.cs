using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>View-model orchestration for the user dashboard's address screens on top of the shared
/// Services.IUserAddressService Api wrapper (/api/addresses, always scoped to the logged-in user).</summary>
public interface IUserAddressPageService
{
    /// <summary>Gets the current user's addresses for the dashboard list.</summary>
    Task<AddressListPageViewModel> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates a new address for the current user.</summary>
    Task<ApiEnvelope> CreateAsync(CreateUserAddressViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing address for the current user.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateUserAddressViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes an address for the current user.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
