using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.LegalPolicyModels;

/// <summary>Bound by LegalPolicies; one row, created on first save.</summary>
public class LegalPolicyFormModel
{
    public string? PrivacyPolicy { get; set; }
    public string? TermsOfService { get; set; }

    public void Load(LegalPolicyResponse policy)
    {
        PrivacyPolicy = policy.PrivacyPolicy;
        TermsOfService = policy.TermsOfService;
    }
}
