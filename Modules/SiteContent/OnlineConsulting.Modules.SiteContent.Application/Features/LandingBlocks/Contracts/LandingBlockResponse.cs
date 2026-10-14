using OnlineConsulting.Modules.SiteContent.Domain.Landing;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;

public record LandingBlockResponse(Guid Id, string Section, string Title, string Body, string? Icon, int DisplayOrder)
{
    public static LandingBlockResponse FromDomain(LandingBlock entity) =>
        new(entity.Id, entity.Section, entity.Title, entity.Body, entity.Icon, entity.DisplayOrder);
}
