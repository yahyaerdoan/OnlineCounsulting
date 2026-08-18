using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.SharedKernel.Notifications;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RegisterDeviceToken;

/// <summary>UserId is always resolved server-side from the authenticated caller, never trusted from the client. Idempotent - the mobile app calls this on every app start/login, IDeviceTokenRepository.RegisterAsync upserts by Token.</summary>
public record RegisterDeviceTokenCommand(Guid UserId, string Token, string Platform) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class RegisterDeviceTokenHandler(IDeviceTokenRepository repository) : IRequestHandler<RegisterDeviceTokenCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        await repository.RegisterAsync(request.UserId, request.Token, request.Platform, cancellationToken);

        return Result.Success("Device token registered successfully.");
    }
}
