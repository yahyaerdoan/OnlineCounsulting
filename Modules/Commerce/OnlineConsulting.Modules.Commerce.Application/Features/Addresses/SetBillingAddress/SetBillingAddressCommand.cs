using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.SetBillingAddress;

public record SetBillingAddressCommand(Guid UserId, Guid AddressId) : IRequest<OperationResult>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class SetBillingAddressHandler(IUserAddressRepository repository) : IRequestHandler<SetBillingAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetBillingAddressCommand request, CancellationToken cancellationToken)
    {
        var newAddress = await repository.GetAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (newAddress is null)
            return Result.NotFound($"Address {request.AddressId} was not found.");

        var oldAddress = await repository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, cancellationToken: cancellationToken);
        if (oldAddress is not null && oldAddress.Id != newAddress.Id)
        {
            oldAddress.IsBillingAddress = false;
            await repository.UpdateAsync(oldAddress);
        }

        newAddress.IsBillingAddress = true;
        await repository.UpdateAsync(newAddress);

        return Result.Success("Billing address set successfully.");
    }
}
