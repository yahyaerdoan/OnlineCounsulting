using FluentValidation;
using OnlineConsulting.Modules.SiteContent.Application.Common;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.LegalPolicies.SaveLegalPolicy;

public class SaveLegalPolicyValidator : AbstractValidator<SaveLegalPolicyCommand>
{
    public SaveLegalPolicyValidator()
    {
        _ = RuleFor(x => x.PrivacyPolicy).MaximumLength(LegalLimits.Document);
        _ = RuleFor(x => x.TermsOfService).MaximumLength(LegalLimits.Document);
    }
}
