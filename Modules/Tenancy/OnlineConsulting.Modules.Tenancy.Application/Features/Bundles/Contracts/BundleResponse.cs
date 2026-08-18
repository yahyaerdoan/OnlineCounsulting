using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;

public record BundleResponse(string Name, List<string> ModuleKeys)
{
    public static BundleResponse FromDomain(Bundle entity) => new(entity.Name, entity.ModuleKeys);
}
