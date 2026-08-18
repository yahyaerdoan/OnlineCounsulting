using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.SharedKernel.Notifications;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RemoveDeviceToken;

/// <summary>Called on logout / push opt-out, so a signed-out device stops receiving pushes.</summary>
public record RemoveDeviceTokenCommand(string Token) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class RemoveDeviceTokenHandler(IDeviceTokenRepository repository) : IRequestHandler<RemoveDeviceTokenCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        await repository.RemoveAsync(request.Token, cancellationToken);

        return Result.Success("Device token removed successfully.");
    }
}
