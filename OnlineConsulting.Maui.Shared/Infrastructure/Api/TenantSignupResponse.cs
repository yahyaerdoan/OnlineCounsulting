namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/v1/tenancy/signup's response shape; SiteUrl is the new business's own site.</summary>
public record TenantSignupResponse(Guid TenantId, string SiteUrl);
