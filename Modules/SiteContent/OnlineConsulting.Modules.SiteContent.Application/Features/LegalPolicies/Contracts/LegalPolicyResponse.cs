using OnlineConsulting.Modules.SiteContent.Domain.Legal;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.Contracts;

public record LegalPolicyResponse(string? PrivacyPolicy, string? TermsOfService, DateTimeOffset UpdatedDate)
{
    public static LegalPolicyResponse FromDomain(LegalPolicy entity) => new(entity.PrivacyPolicy, entity.TermsOfService, entity.UpdatedDate ?? entity.CreatedDate);
}
