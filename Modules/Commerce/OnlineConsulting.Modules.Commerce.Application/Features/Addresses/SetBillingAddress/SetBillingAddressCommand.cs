using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetBillingAddress;

public record SetBillingAddressCommand(Guid UserId, Guid AddressId) : IRequest<OperationResult>, ICommerceTransactionRequest, ISecureAddRequest
{
    public string[] Roles => [];
}

public class SetBillingAddressHandler(IUserAddressRepository repository) : IRequestHandler<SetBillingAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetBillingAddressCommand request, CancellationToken cancellationToken)
    {
        var newAddress = await repository.GetAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (newAddress is null)
        {
            return Result.NotFound($"Address {request.AddressId} was not found.");
        }

        await UserAddressDefaultFlag.ClearPreviousBillingHolderAsync(repository, request.UserId, newAddress.Id, cancellationToken);

        newAddress.IsBillingAddress = true;
        _ = await repository.UpdateAsync(newAddress, cancellationToken: cancellationToken);

        return Result.Success("Billing address set successfully.");
    }
}
