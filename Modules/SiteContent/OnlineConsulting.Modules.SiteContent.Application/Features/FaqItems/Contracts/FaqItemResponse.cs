using OnlineConsulting.Modules.SiteContent.Domain;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.Contracts;

public record FaqItemResponse(Guid Id, Guid ServiceId, string Question, string Answer, int DisplayOrder)
{
    public static FaqItemResponse FromDomain(FaqItem entity) => new(entity.Id, entity.ServiceId, entity.Question, entity.Answer, entity.DisplayOrder);
}
