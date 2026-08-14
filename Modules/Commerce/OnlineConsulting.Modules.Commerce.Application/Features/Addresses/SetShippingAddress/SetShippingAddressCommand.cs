using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetShippingAddress;

public record SetShippingAddressCommand(Guid UserId, Guid AddressId) : IRequest<OperationResult>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.User];
}

public class SetShippingAddressHandler(IUserAddressRepository repository) : IRequestHandler<SetShippingAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetShippingAddressCommand request, CancellationToken cancellationToken)
    {
        var newAddress = await repository.GetAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (newAddress is null)
            return Result.NotFound($"Address {request.AddressId} was not found.");

        var oldAddress = await repository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, cancellationToken: cancellationToken);
        if (oldAddress is not null && oldAddress.Id != newAddress.Id)
        {
            oldAddress.IsShippingAddress = false;
            await repository.UpdateAsync(oldAddress);
        }

        newAddress.IsShippingAddress = true;
        await repository.UpdateAsync(newAddress);

        return Result.Success("Shipping address set successfully.");
    }
}
