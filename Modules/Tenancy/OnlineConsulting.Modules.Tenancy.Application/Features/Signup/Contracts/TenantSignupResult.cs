namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup.Contracts;

/// <summary>SiteUrl is the new tenant's own site, where its admin signs in.</summary>
public record TenantSignupResult(Guid TenantId, string SiteUrl);
