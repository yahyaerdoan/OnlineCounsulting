using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;

/// <summary>
/// Updates a user address. Setting <see cref="IsShippingAddress"/> or <see cref="IsBillingAddress"/>
/// unsets that flag on the user's previous holder, since each user has at most one of each.
/// </summary>
public record UpdateUserAddressCommand(Guid Id, Guid UserId, string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress)
    : IRequest<OperationResult>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class UpdateUserAddressHandler(IUserAddressRepository repository) : IRequestHandler<UpdateUserAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await repository.GetAsync(a => a.Id == request.Id && a.UserId == request.UserId, cancellationToken: cancellationToken);

        if (address is null)
        {
            return Result.NotFound($"Address {request.Id} was not found.");
        }

        if (request.IsShippingAddress && !address.IsShippingAddress)
        {
            await UserAddressDefaultFlag.ClearPreviousShippingHolderAsync(repository, request.UserId, address.Id, cancellationToken);
        }

        if (request.IsBillingAddress && !address.IsBillingAddress)
        {
            await UserAddressDefaultFlag.ClearPreviousBillingHolderAsync(repository, request.UserId, address.Id, cancellationToken);
        }

        address.AddressName = request.AddressName;
        address.CompanyName = request.CompanyName;
        address.Country = request.Country;
        address.AddressLine = request.AddressLine;
        address.City = request.City;
        address.State = request.State;
        address.Zipcode = request.Zipcode;
        address.Notes = request.Notes;
        address.IsShippingAddress = request.IsShippingAddress;
        address.IsBillingAddress = request.IsBillingAddress;

        _ = await repository.UpdateAsync(address);

        return Result.Success("Address updated successfully.");
    }
}
