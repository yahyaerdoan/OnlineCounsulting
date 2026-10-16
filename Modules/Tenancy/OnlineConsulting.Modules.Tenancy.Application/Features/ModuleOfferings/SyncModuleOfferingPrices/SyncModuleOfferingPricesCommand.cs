using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Constants;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.SyncModuleOfferingPrices;

/// <summary>Re-mints provider prices missing on the active provider (wiped test data, provider switch); returns how many were re-minted.</summary>
public record SyncModuleOfferingPricesCommand : IRequest<OperationDataResult<int>>, ISecureAddRequest
{
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Platform-level only.</summary>
    public bool AllowTenantBypass => false;
}

public class SyncModuleOfferingPricesHandler(IModuleOfferingRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<SyncModuleOfferingPricesCommand, OperationDataResult<int>>
{
    public async Task<OperationDataResult<int>> Handle(SyncModuleOfferingPricesCommand request, CancellationToken cancellationToken)
    {
        var offerings = await repository.GetAllAsync(cancellationToken: cancellationToken);
        var synced = 0;

        foreach (var offering in offerings)
        {
            if (offering.ProviderPriceId is { } priceId && await subscriptionGateway.PriceExistsAsync(priceId, cancellationToken: cancellationToken))
            {
                continue;
            }

            var priceResult = await subscriptionGateway
                .EnsurePriceAsync(new EnsurePriceRequest(offering.Id.ToString(), offering.Name, offering.Price, "usd", offering.BillingCycle), cancellationToken);

            offering.ProviderProductId = priceResult.ProviderProductId;
            offering.ProviderPriceId = priceResult.ProviderPriceId;

            _ = await repository.UpdateAsync(offering, cancellationToken: cancellationToken);
            synced++;
        }

        return Result.Success(synced, string.Format(ModuleOfferingMessages.PricesSyncedFormat, synced));
    }
}
