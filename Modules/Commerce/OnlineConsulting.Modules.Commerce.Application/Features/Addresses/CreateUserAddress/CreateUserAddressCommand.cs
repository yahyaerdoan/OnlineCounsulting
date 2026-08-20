using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
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
        // A user has at most one shipping and one billing address, so setting this one unsets the old holder of that flag.
        if (request.IsShippingAddress)
        {
            var oldShipping = await repository.GetAsync(a => a.UserId == request.UserId && a.IsShippingAddress, cancellationToken: cancellationToken);
            if (oldShipping is not null)
            {
                oldShipping.IsShippingAddress = false;
                _ = await repository.UpdateAsync(oldShipping);
            }
        }

        if (request.IsBillingAddress)
        {
            var oldBilling = await repository.GetAsync(a => a.UserId == request.UserId && a.IsBillingAddress, cancellationToken: cancellationToken);
            if (oldBilling is not null)
            {
                oldBilling.IsBillingAddress = false;
                _ = await repository.UpdateAsync(oldBilling);
            }
        }

        var address = new UserAddress
        {
            Id = Guid.NewGuid(),
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

        return Result.Created(address.Id, "Address created successfully.");
    }
}
