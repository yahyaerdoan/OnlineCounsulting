using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using ResultHandler.Core.Base;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;

/// <summary>Upserts the current tenant's override for a key. Single write (Add or Update, never both) - deliberately not ITransactionAddRequest, reserved for handlers with 2+ SaveChanges calls. ICacheRemoveRequest.CacheKey is empty (matches MetroMiles' CacheRemovingSecuredRequest convention) - this command has no single cache entry of its own to remove, it only needs to clear GetFeatureFlagsQuery's CacheGroupKey.</summary>
public record SetFeatureFlagCommand(string Key, bool IsEnabled) : IRequest<OperationResult>, ISecureAddRequest, ICacheRemoveRequest
{
    [JsonIgnore]
    public Guid TenantId { get; init; }

    [JsonIgnore]
    public string[] Roles => [FeatureFlagsOperationClaims.Admin, FeatureFlagsOperationClaims.Update];

    [JsonIgnore]
    public string CacheKey => string.Empty;

    [JsonIgnore]
    public bool ByPassCache => false;

    [JsonIgnore]
    public string? CacheGroupKey => $"FeatureFlags:{TenantId}";
}

public class SetFeatureFlagHandler(FeatureFlagUpserter upserter)
    : IRequestHandler<SetFeatureFlagCommand, OperationResult>
{
    public Task<OperationResult> Handle(SetFeatureFlagCommand request, CancellationToken cancellationToken) =>
        upserter.UpsertAsync(request.Key, request.IsEnabled, cancellationToken);
}
