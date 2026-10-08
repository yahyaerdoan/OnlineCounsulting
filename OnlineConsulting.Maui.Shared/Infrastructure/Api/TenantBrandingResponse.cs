namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/tenancy/branding.</summary>
public record TenantBrandingResponse(string Name, Guid? LogoMediaAssetId, bool IsPlatform, string PlatformUrl);
