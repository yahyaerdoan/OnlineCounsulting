namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>The business name and logo the caller's site shows; the platform's own for the default tenant. IsPlatform marks the platform's own site,
/// and PlatformUrl is where a business site sends visitors who want the platform (pricing, signup).</summary>
public record TenantBrandingResponse(string Name, Guid? LogoMediaAssetId, bool IsPlatform, string PlatformUrl);
