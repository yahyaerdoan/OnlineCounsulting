using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.UpdateUserAddress;

public record UpdateUserAddressCommand(Guid Id, Guid UserId, string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress)
    : IRequest<OperationResult>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.User];
}

public class UpdateUserAddressHandler(IUserAddressRepository repository) : IRequestHandler<UpdateUserAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await repository.GetAsync(a => a.Id == request.Id && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (address is null)
            return Result.NotFound($"Address {request.Id} was not found.");

        // See CreateUserAddressCommand for why claiming shipping/billing here means unsetting
        // whichever other address currently holds that flag.
        if (request.IsShippingAddress && !address.IsShippingAddress)
        {
            var oldShipping = await repository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, cancellationToken: cancellationToken);
            if (oldShipping is not null)
            {
                oldShipping.IsShippingAddress = false;
                await repository.UpdateAsync(oldShipping);
            }
        }

        if (request.IsBillingAddress && !address.IsBillingAddress)
        {
            var oldBilling = await repository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, cancellationToken: cancellationToken);
            if (oldBilling is not null)
            {
                oldBilling.IsBillingAddress = false;
                await repository.UpdateAsync(oldBilling);
            }
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

        await repository.UpdateAsync(address);

        return Result.Success("Address updated successfully.");
    }
}
