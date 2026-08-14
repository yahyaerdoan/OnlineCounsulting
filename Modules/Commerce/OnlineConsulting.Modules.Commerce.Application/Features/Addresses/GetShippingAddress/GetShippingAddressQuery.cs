using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetShippingAddress;

public record GetShippingAddressQuery(Guid UserId) : IRequest<OperationDataResult<UserAddressResponse>>;

public class GetShippingAddressHandler(IUserAddressRepository repository) : IRequestHandler<GetShippingAddressQuery, OperationDataResult<UserAddressResponse>>
{
    public async Task<OperationDataResult<UserAddressResponse>> Handle(GetShippingAddressQuery request, CancellationToken cancellationToken)
    {
        // Read-only lookup - no mutation follows, so track nothing.
        var address = await repository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, enableTracking: false, cancellationToken: cancellationToken);

        return address is null
            ? Result.NotFound<UserAddressResponse>(AddressMessages.ShippingAddressNotFound)
            : Result.Success(UserAddressResponse.FromDomain(address), "Shipping address retrieved successfully.");
    }
}
