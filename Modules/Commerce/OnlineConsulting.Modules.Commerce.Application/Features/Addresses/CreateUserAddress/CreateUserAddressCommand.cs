using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Common;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.CreateUserAddress;

public record CreateUserAddressCommand(Guid UserId, string AddressName, string? CompanyName, string Country, string AddressLine, string City, string State, string Zipcode, string? Notes, bool IsShippingAddress, bool IsBillingAddress)
    : IRequest<OperationDataResult<Guid>>, ITransactionAddRequest, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class CreateUserAddressHandler(IUserAddressRepository repository) : IRequestHandler<CreateUserAddressCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new UserAddress
        {
            UserId = request.UserId,
            AddressName = request.AddressName,
            CompanyName = request.CompanyName,
            Country = request.Country,
            AddressLine = request.AddressLine,
            City = request.City,
            State = request.State,
            Zipcode = request.Zipcode,
            Notes = request.Notes,
            IsShippingAddress = request.IsShippingAddress,
            IsBillingAddress = request.IsBillingAddress,
        };
        _ = await repository.AddAsync(address);

        // A user has at most one shipping and one billing address, so claiming either here unsets its old holder.
        // Uses the id the insert above just assigned, not a pre-generated one - see UserAddressDefaultFlag.
        if (request.IsShippingAddress)
        {
            await UserAddressDefaultFlag.ClearPreviousShippingHolderAsync(repository, request.UserId, address.Id, cancellationToken);
        }

        if (request.IsBillingAddress)
        {
            await UserAddressDefaultFlag.ClearPreviousBillingHolderAsync(repository, request.UserId, address.Id, cancellationToken);
        }

        return Result.Created(address.Id, "Address created successfully.");
    }
}
