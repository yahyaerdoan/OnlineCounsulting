namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Body of PUT /api/v1/tenancy/my-tenant/branding.</summary>
public record UpdateTenantBrandingRequest(string Name, Guid? LogoMediaAssetId);
