namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>What customers see as the business: its name and optional logo.</summary>
public record TenantBrand(string Name, Guid? LogoMediaAssetId);
