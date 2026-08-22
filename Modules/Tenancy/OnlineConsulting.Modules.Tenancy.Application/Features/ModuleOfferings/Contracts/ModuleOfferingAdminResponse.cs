using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;

public record ModuleOfferingAdminResponse(
    Guid Id,
    string Key,
    string Name,
    decimal Price,
    string BillingCycle,
    bool IsPubliclyVisible,
    string? ProviderProductId,
    string? ProviderPriceId)
{
    public static ModuleOfferingAdminResponse FromDomain(ModuleOffering entity) => new(
        entity.Id, entity.Key, entity.Name, entity.Price, entity.BillingCycle, entity.IsPubliclyVisible,
        entity.ProviderProductId, entity.ProviderPriceId);
}
