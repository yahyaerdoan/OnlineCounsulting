using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Contracts;

public record ServiceAreaResponse(Guid Id, string Name, string State, string Slug, string? IntroText, int DisplayOrder)
{
    public static ServiceAreaResponse FromDomain(ServiceArea entity) => new(entity.Id, entity.Name, entity.State, entity.Slug, entity.IntroText, entity.DisplayOrder);
}
