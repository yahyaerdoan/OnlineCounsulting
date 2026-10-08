using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Cachings.Abstractions;
using MediatR;
using OnlineConsulting.Modules.FeatureFlags.Application.Common;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;

namespace OnlineConsulting.Modules.FeatureFlags.Application.Features.FeatureFlags.SetFeatureFlag;

/// <summary>Upserts the current tenant's flag; not IFeatureFlagsTransactionRequest (single SaveChanges), and CacheKey is empty since only the CacheGroupKey needs clearing.</summary>
public record SetFeatureFlagCommand(string Key, bool IsEnabled) : IRequest<OperationResult>, ISecureAddRequest, ICacheRemoveRequest
{
    public Guid TenantId { get; init; }

    public string[] Roles => [FeatureFlagsOperationClaims.Admin, FeatureFlagsOperationClaims.Update];

    public string CacheKey => string.Empty;

    public bool ByPassCache => false;

    public string? CacheGroupKey => $"FeatureFlags:{TenantId}";
}

public class SetFeatureFlagHandler(FeatureFlagUpserter upserter) : IRequestHandler<SetFeatureFlagCommand, OperationResult>
{
    public Task<OperationResult> Handle(SetFeatureFlagCommand request, CancellationToken cancellationToken) =>
        upserter.UpsertAsync(request.Key, request.IsEnabled, cancellationToken);
}
