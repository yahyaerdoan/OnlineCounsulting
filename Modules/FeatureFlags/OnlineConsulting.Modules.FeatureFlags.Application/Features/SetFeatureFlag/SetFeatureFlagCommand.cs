using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Contracts;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Rules;
using OnlineConsulting.Modules.FeatureFlags.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.SetFeatureFlag;

/// <summary>Upserts the current tenant's override for a key. Single write (Add or Update, never both) - deliberately not ITransactionAddRequest, reserved for handlers with 2+ SaveChanges calls. ICacheRemoveRequest.CacheKey is empty (matches MetroMiles' CacheRemovingSecuredRequest convention) - this command has no single cache entry of its own to remove, it only needs to clear GetFeatureFlagsQuery's CacheGroupKey.</summary>
public record SetFeatureFlagCommand(string Key, bool IsEnabled) : IRequest<OperationResult>, ISecureAddRequest, ICacheRemoveRequest
{
    [JsonIgnore]
    public Guid TenantId { get; init; }

    [JsonIgnore]
    public string[] Roles => [FeatureFlagsOperationClaims.Admin];

    [JsonIgnore]
    public string CacheKey => string.Empty;

    [JsonIgnore]
    public bool ByPassCache => false;

    [JsonIgnore]
    public string? CacheGroupKey => $"FeatureFlags:{TenantId}";
}

public class SetFeatureFlagHandler(IFeatureFlagRepository repository, IFeatureFlagCacheInvalidator cacheInvalidator)
    : IRequestHandler<SetFeatureFlagCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SetFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        if (!FeatureFlagKeys.Defaults.ContainsKey(request.Key))
            return FeatureFlagBusinessRules.UnknownKey(request.Key);

        var existing = await repository.GetAsync(f => f.Key == request.Key, cancellationToken: cancellationToken);

        if (existing is null)
        {
            await repository.AddAsync(new FeatureFlag { Id = Guid.NewGuid(), Key = request.Key, IsEnabled = request.IsEnabled });
        }
        else
        {
            existing.IsEnabled = request.IsEnabled;
            await repository.UpdateAsync(existing);
        }

        // Invalidates IFeatureFlagReader's own IMemoryCache (the cross-module hot-path reader, outside MediatR entirely - see FeatureFlagCache.cs). CacheRemovingBehavior separately clears GetFeatureFlagsQuery's CacheGroupKey via the ICacheRemoveRequest above; these are two independent caches serving two different call paths, not a duplicate of each other.
        cacheInvalidator.Invalidate();

        return Result.Success("Feature flag updated successfully.");
    }
}
