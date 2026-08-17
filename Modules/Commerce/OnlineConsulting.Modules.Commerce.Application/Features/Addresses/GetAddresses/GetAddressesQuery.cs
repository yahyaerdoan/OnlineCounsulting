using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetAddresses;

public record GetAddressesQuery(Guid UserId) : IRequest<OperationDataResult<List<UserAddressResponse>>>;

public class GetAddressesHandler(IUserAddressRepository repository) : IRequestHandler<GetAddressesQuery, OperationDataResult<List<UserAddressResponse>>>
{
    public async Task<OperationDataResult<List<UserAddressResponse>>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await repository.GetListAsync(a => a.UserId == request.UserId, cancellationToken: cancellationToken);

        List<UserAddressResponse> responses = [.. addresses.Items.Select(UserAddressResponse.FromDomain)];

        return Result.Success(responses, "Addresses retrieved successfully.");
    }
}
