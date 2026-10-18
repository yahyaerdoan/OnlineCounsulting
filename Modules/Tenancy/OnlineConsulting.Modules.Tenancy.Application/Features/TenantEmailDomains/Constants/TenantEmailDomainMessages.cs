namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;

public static class TenantEmailDomainMessages
{
    public const string Retrieved = "Email domain retrieved.";
    public const string NotSet = "No email domain is set up yet.";
    public const string AlreadySet = "Your business already has an email domain. Remove it first to use another one.";
    public const string DomainTaken = "This domain is already in use.";
    public const string InvalidDomain = "Enter a domain like yourbusiness.com, without https:// or www.";
    public const string ProviderFailed = "We couldn't reach the email provider right now. Please try again in a few minutes.";
    public const string Added = "Domain added. Add the DNS records below at your domain provider.";
    public const string Live = "Emails now go out from your domain.";
    public const string StillPending = "Your DNS records aren't in place yet. It can take a few hours after you add them.";
    public const string Removed = "Email domain removed. Emails go out from our address again.";
}
