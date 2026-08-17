using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

/// <summary>View-model orchestration for the user dashboard's address screens on top of the shared
/// Services.IUserAddressService Api wrapper (/api/addresses, always scoped to the logged-in user).</summary>
public interface IUserAddressPageService
{
    Task<AddressListPageViewModel> GetListAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateUserAddressViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateUserAddressViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
