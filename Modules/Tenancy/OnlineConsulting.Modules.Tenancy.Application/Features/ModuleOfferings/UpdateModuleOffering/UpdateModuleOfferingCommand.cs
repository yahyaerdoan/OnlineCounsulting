using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Constants;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.UpdateModuleOffering;

/// <summary>Only local fields - never touches Key/Price/BillingCycle/ProviderProductId/ProviderPriceId. Provider prices are immutable and Key is what every TenantSubscriptionItem/FeatureFlag already points to, so a real price or key change requires creating a new offering (out of scope for this phase), mirroring UpdateMembershipPlanCommand.</summary>
public record UpdateModuleOfferingCommand(Guid Id, string Name, bool IsPubliclyVisible) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class UpdateModuleOfferingHandler(IModuleOfferingRepository repository) : IRequestHandler<UpdateModuleOfferingCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateModuleOfferingCommand request, CancellationToken cancellationToken)
    {
        var offering = await repository.GetAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
        if (offering is null)
        {
            return Result.NotFound(string.Format(ModuleOfferingMessages.ModuleOfferingNotFoundFormat, request.Id));
        }

        offering.Name = request.Name;
        offering.IsPubliclyVisible = request.IsPubliclyVisible;

        _ = await repository.UpdateAsync(offering);

        return Result.Success("Module offering updated successfully.");
    }
}
