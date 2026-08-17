using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Contracts;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.GetFeatureFlags;

/// <summary>Every known key (FeatureFlagKeys.Defaults) merged with the tenant's stored overrides, so the admin UI always sees a full, stable list even before any override row exists. Cached via Core.ApplicationLayer's ICacheAddRequest (same CacheAddingBehavior/CacheGroupKey pattern MetroMiles uses for GetListBrandQuery) - TenantId has to be a plain field rather than resolved from ITenantProvider inside CacheKey, since these are computed properties with no DI access; the endpoint fills it in from the request's tenant context, same as CreatedBrandCommand-style server-computed fields.</summary>
public record GetFeatureFlagsQuery(Guid TenantId) : IRequest<OperationDataResult<List<FeatureFlagResponse>>>, ISecureAddRequest, ICacheAddRequest
{
    [JsonIgnore]
    public string[] Roles => [FeatureFlagsOperationClaims.Admin];

    [JsonIgnore]
    public string CacheKey => $"GetFeatureFlags({TenantId})";

    [JsonIgnore]
    public bool ByPassCache => false;

    /// <summary>Short-lived - this is an admin settings screen, not a high-traffic list, so a long TTL isn't worth the staleness risk. SetFeatureFlagCommand's matching CacheGroupKey clears this immediately on write anyway; this is only a safety net.</summary>
    [JsonIgnore]
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(5);

    [JsonIgnore]
    public string? CacheGroupKey => $"FeatureFlags:{TenantId}";
}

public class GetFeatureFlagsHandler(IFeatureFlagRepository repository)
    : IRequestHandler<GetFeatureFlagsQuery, OperationDataResult<List<FeatureFlagResponse>>>
{
    public async Task<OperationDataResult<List<FeatureFlagResponse>>> Handle(GetFeatureFlagsQuery request, CancellationToken cancellationToken)
    {
        var overrides = await repository.GetListAsync(size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        var overridesByKey = overrides.Items.ToDictionary(f => f.Key, f => f.IsEnabled);

        var response = FeatureFlagKeys.Defaults
            .Select(kvp => new FeatureFlagResponse(kvp.Key, overridesByKey.GetValueOrDefault(kvp.Key, kvp.Value)))
            .ToList();

        return Result.Success(response, "Feature flags retrieved successfully.");
    }
}
