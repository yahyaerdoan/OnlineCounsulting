using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Constants;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetBillingAddress;

public record GetBillingAddressQuery(Guid UserId) : IRequest<OperationDataResult<UserAddressResponse>>;

public class GetBillingAddressHandler(IUserAddressRepository repository)
    : IRequestHandler<GetBillingAddressQuery, OperationDataResult<UserAddressResponse>>
{
    public async Task<OperationDataResult<UserAddressResponse>> Handle(GetBillingAddressQuery request, CancellationToken cancellationToken)
    {
        // Read-only lookup - no mutation follows, so track nothing.
        var address = await repository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, enableTracking: false, cancellationToken: cancellationToken);

        return address is null
            ? Result.NotFound<UserAddressResponse>(AddressMessages.BillingAddressNotFound)
            : Result.Success(UserAddressResponse.FromDomain(address), "Billing address retrieved successfully.");
    }
}
