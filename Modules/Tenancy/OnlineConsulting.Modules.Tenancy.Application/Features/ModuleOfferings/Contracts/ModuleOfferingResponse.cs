using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Contracts;

public record ModuleOfferingResponse(string Key, string Name, decimal Price, string BillingCycle)
{
    public static ModuleOfferingResponse FromDomain(ModuleOffering entity) => new(entity.Key, entity.Name, entity.Price, entity.BillingCycle);
}
