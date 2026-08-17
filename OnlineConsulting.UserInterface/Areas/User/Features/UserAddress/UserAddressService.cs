using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;

public class UserAddressService(IApiClient apiClient) : IUserAddressService
{
    private const string AddressesPath = "/api/addresses";

    public async Task<List<UserAddressResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<UserAddressResponse>>(AddressesPath, cancellationToken);
        return result.ResultData ?? [];
    }

    public async Task<UserAddressResponse?> GetBillingAsync(CancellationToken cancellationToken = default) =>
        (await apiClient.GetAsync<UserAddressResponse>($"{AddressesPath}/billing", cancellationToken)).ResultData;

    public async Task<UserAddressResponse?> GetShippingAsync(CancellationToken cancellationToken = default) =>
        (await apiClient.GetAsync<UserAddressResponse>($"{AddressesPath}/shipping", cancellationToken)).ResultData;

    public Task<ApiEnvelope<Guid>> CreateAsync(string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<Guid>(AddressesPath, BuildCommand(addressName, companyName, country, addressLine, city, state, zipcode, notes, isShippingAddress, isBillingAddress), cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(Guid id, string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{AddressesPath}/{id}", BuildCommand(addressName, companyName, country, addressLine, city, state, zipcode, notes, isShippingAddress, isBillingAddress), cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{AddressesPath}/{id}", cancellationToken);

    public Task<ApiEnvelope> SetBillingAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{AddressesPath}/{id}/billing", null, cancellationToken);

    public Task<ApiEnvelope> SetShippingAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{AddressesPath}/{id}/shipping", null, cancellationToken);

    private static object BuildCommand(string addressName, string? companyName, string country, string addressLine, string city, string state, string zipcode, string? notes, bool isShippingAddress, bool isBillingAddress) => new
    {
        AddressName = addressName,
        CompanyName = companyName,
        Country = country,
        AddressLine = addressLine,
        City = city,
        State = state,
        Zipcode = zipcode,
        Notes = notes,
        IsShippingAddress = isShippingAddress,
        IsBillingAddress = isBillingAddress,
    };
}
