using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Abstractions;
using OnlineConsulting.Modules.FeatureFlags.Application.Contracts;
using OnlineConsulting.Modules.FeatureFlags.Application.Features.Constants;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.GetFeatureFlags;

/// <summary>Merges every known key (FeatureFlagKeys.Defaults) with the tenant's stored overrides so the admin UI always sees a full list; TenantId is a plain field since CacheKey has no DI access to ITenantProvider.</summary>
public record GetFeatureFlagsQuery(Guid TenantId) : IRequest<OperationDataResult<List<FeatureFlagResponse>>>, ISecureAddRequest, ICacheAddRequest
{
    [JsonIgnore]
    public string[] Roles => [FeatureFlagsOperationClaims.Admin, FeatureFlagsOperationClaims.Read];

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

public class GetFeatureFlagsHandler(IFeatureFlagRepository repository, ITenantModulePricingReader tenantModulePricingReader) : IRequestHandler<GetFeatureFlagsQuery, OperationDataResult<List<FeatureFlagResponse>>>
{
    public async Task<OperationDataResult<List<FeatureFlagResponse>>> Handle(GetFeatureFlagsQuery request, CancellationToken cancellationToken)
    {
        var overrides = await repository.GetListAsync(orderBy: q => q.OrderBy(f => f.Id), size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);

        var overridesByKey = overrides.Items.ToDictionary(f => f.Key, f => f.IsEnabled);

        var pricingByKey = await tenantModulePricingReader.GetForTenantAsync(request.TenantId, cancellationToken);

        var response = FeatureFlagKeys.Defaults
            .Select(kvp =>
            {
                var isPurchased = pricingByKey.TryGetValue(kvp.Key, out var pricing);

                return new FeatureFlagResponse(
                    kvp.Key,
                    overridesByKey.GetValueOrDefault(kvp.Key, kvp.Value),
                    isPurchased ? pricing.Price : null,
                    isPurchased);
            })
            .ToList();

        return Result.Success(response, "Feature flags retrieved successfully.");
    }
}
