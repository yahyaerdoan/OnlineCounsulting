using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.DeleteUserAddress;

public record DeleteUserAddressCommand(Guid Id, Guid UserId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.User];
}

public class DeleteUserAddressHandler(IUserAddressRepository repository) : IRequestHandler<DeleteUserAddressCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await repository.GetAsync(a => a.Id == request.Id && a.UserId == request.UserId, cancellationToken: cancellationToken);
        if (address is null)
            return Result.NotFound($"Address {request.Id} was not found.");

        await repository.DeleteAsync(address);

        return Result.Success("Address deleted successfully.");
    }
}
