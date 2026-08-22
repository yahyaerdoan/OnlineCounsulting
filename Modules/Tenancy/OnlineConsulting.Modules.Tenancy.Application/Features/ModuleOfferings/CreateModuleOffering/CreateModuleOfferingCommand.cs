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

/// <summary>Creates the offering's provider-side product/price before persisting it - prices are immutable on the provider side, so this is the only place that ever mints one for a given offering (see ModuleOffering.ProviderPriceId), mirroring CreateMembershipPlanCommand.</summary>
public record CreateModuleOfferingCommand(string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class CreateModuleOfferingHandler(IModuleOfferingRepository repository, ISubscriptionGateway subscriptionGateway)
    : IRequestHandler<CreateModuleOfferingCommand, OperationDataResult<Guid>>
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
            Id = Guid.NewGuid(),
            Key = request.Key,
            Name = request.Name,
            Price = request.Price,
            BillingCycle = request.BillingCycle,
            IsPubliclyVisible = request.IsPubliclyVisible,
        };

        var priceResult = await subscriptionGateway.EnsurePriceAsync(
            new EnsurePriceRequest(offering.Id.ToString(), offering.Name, offering.Price, "usd", offering.BillingCycle), cancellationToken);
        offering.ProviderProductId = priceResult.ProviderProductId;
        offering.ProviderPriceId = priceResult.ProviderPriceId;

        _ = await repository.AddAsync(offering);

        return Result.Created(offering.Id, "Module offering created successfully.");
    }
}
