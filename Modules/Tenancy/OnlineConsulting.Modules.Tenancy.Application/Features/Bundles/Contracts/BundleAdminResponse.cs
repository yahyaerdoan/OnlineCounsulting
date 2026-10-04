using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Contracts;

public record BundleAdminResponse(Guid Id, string Name, List<string> ModuleKeys, bool IsPubliclyVisible)
{
    public static BundleAdminResponse FromDomain(Bundle entity) => new(entity.Id, entity.Name, entity.ModuleKeys, entity.IsPubliclyVisible);
}
