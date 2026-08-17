using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

public class UserAddressPageService(IUserAddressService userAddressService) : IUserAddressPageService
{
    public async Task<AddressListPageViewModel> GetListAsync(CancellationToken cancellationToken = default)
    {
        var addresses = await userAddressService.GetAllAsync(cancellationToken);
        return new AddressListPageViewModel
        {
            UserAddressList = addresses.Select(a => new UserAddressListItemViewModel(
                a.Id, a.AddressName, a.CompanyName, a.Country, a.AddressLine, a.City, a.State, a.Zipcode,
                a.Notes, a.IsShippingAddress, a.IsBillingAddress)).ToList(),
        };
    }

    public async Task<ApiEnvelope> CreateAsync(CreateUserAddressViewModel model, CancellationToken cancellationToken = default) =>
        (await userAddressService.CreateAsync(model.AddressName, model.CompanyName, model.Country, model.AddressLine,
            model.City, model.State, model.Zipcode, model.Notes, model.IsShippingAddress, model.IsBillingAddress,
            cancellationToken)).WithoutData();

    public Task<ApiEnvelope> UpdateAsync(UpdateUserAddressViewModel model, CancellationToken cancellationToken = default) =>
        userAddressService.UpdateAsync(model.Id, model.AddressName, model.CompanyName, model.Country, model.AddressLine,
            model.City, model.State, model.Zipcode, model.Notes, model.IsShippingAddress, model.IsBillingAddress,
            cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        userAddressService.DeleteAsync(id, cancellationToken);
}
