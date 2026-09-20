using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetShippingAddress;

public record SetShippingAddressCommand(Guid UserId, Guid AddressId) : IRequest<OperationResult>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class SetShippingAddressHandler(IUserAddressRepository repository) : IRequestHandler<SetShippingAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetShippingAddressCommand request, CancellationToken cancellationToken)
    {
        var newAddress = await repository.GetAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (newAddress is null)
        {
            return Result.NotFound($"Address {request.AddressId} was not found.");
        }

        await UserAddressDefaultFlag.ClearPreviousShippingHolderAsync(repository, request.UserId, newAddress.Id, cancellationToken);

        newAddress.IsShippingAddress = true;
        _ = await repository.UpdateAsync(newAddress);

        return Result.Success("Shipping address set successfully.");
    }
}
