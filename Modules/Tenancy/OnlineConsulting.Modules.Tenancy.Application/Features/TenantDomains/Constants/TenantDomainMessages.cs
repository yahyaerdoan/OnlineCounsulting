namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;

public static class TenantDomainMessages
{
    public const string Retrieved = "Custom domain retrieved.";
    public const string NotSet = "No custom domain is set up yet.";
    public const string AlreadySet = "Your business already has a custom domain. Remove it first to use another one.";
    public const string HostTaken = "This domain is already in use.";
    public const string PlatformHost = "This address is part of our platform. Enter a domain you own.";
    public const string InvalidHost = "Enter a domain like www.example.com, without https://.";
    public const string ProviderFailed = "We couldn't reach the hosting provider right now. Please try again in a few minutes.";
    public const string Added = "Domain added. Add the DNS records below at your domain provider.";
    public const string Live = "Your domain is live.";
    public const string StillPending = "Your DNS records aren't in place yet. It can take a few hours after you add them.";
    public const string Removed = "Custom domain removed.";
}
