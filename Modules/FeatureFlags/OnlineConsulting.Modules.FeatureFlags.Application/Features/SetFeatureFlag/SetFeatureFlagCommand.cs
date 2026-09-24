using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using ResultHandler.Core.Base;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;

/// <summary>Upserts the current tenant's flag; not ITransactionAddRequest (single SaveChanges), and CacheKey is empty since only the CacheGroupKey needs clearing.</summary>
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

public class SetFeatureFlagHandler(FeatureFlagUpserter upserter) : IRequestHandler<SetFeatureFlagCommand, OperationResult>
{
    public Task<OperationResult> Handle(SetFeatureFlagCommand request, CancellationToken cancellationToken) =>
        upserter.UpsertAsync(request.Key, request.IsEnabled, cancellationToken);
}
