using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Constants;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Payments;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.CreateModuleOffering;

/// <summary>Mints the offering's provider-side product/price before persisting - the only place that does, since provider prices are immutable (see ModuleOffering.ProviderPriceId).</summary>
public record CreateModuleOfferingCommand(string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class CreateModuleOfferingHandler(IModuleOfferingRepository repository, ISubscriptionGateway subscriptionGateway) : IRequestHandler<CreateModuleOfferingCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateModuleOfferingCommand request, CancellationToken cancellationToken)
    {
        var keyTaken = await repository.AnyAsync(m => m.Key == request.Key, cancellationToken: cancellationToken);

        if (keyTaken)
        {
            return Result.Conflict<Guid>(ModuleOfferingMessages.KeyAlreadyExists);
        }

        var offering = new ModuleOffering
        {
            Key = request.Key,
            Name = request.Name,
            Price = request.Price,
            BillingCycle = request.BillingCycle,
            IsPubliclyVisible = request.IsPubliclyVisible,
        };

        var priceResult = await subscriptionGateway
            .EnsurePriceAsync(new EnsurePriceRequest(offering.Id.ToString(), offering.Name, offering.Price, "usd", offering.BillingCycle), cancellationToken);

        offering.ProviderProductId = priceResult.ProviderProductId;
        offering.ProviderPriceId = priceResult.ProviderPriceId;

        _ = await repository.AddAsync(offering);

        return Result.Created(offering.Id, "Module offering created successfully.");
    }
}
