namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>What customers see as the business: its name, optional logo, and its own From address once its email domain is authenticated.</summary>
public record TenantBrand(string Name, Guid? LogoMediaAssetId, string? SenderAddress = null);
