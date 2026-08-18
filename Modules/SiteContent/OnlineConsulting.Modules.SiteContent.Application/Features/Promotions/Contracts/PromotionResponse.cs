using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;

public record PromotionResponse(Guid Id, string Title, string Description, string? CtaText, string? CtaUrl, DateTimeOffset? ExpiresAt, int DisplayOrder)
{
    public static PromotionResponse FromDomain(Promotion entity) => new(entity.Id, entity.Title, entity.Description, entity.CtaText, entity.CtaUrl, entity.ExpiresAt, entity.DisplayOrder);
}
