namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;

/// <summary>The business name and logo the caller's site shows; the platform's own for the default tenant. IsPlatform marks the platform's own site,
/// PlatformUrl is where a business site sends visitors who want the platform (pricing, signup), and PlatformName credits it in the footer.</summary>
public record TenantBrandingResponse(string Name, Guid? LogoMediaAssetId, bool IsPlatform, string PlatformUrl, string PlatformName);
