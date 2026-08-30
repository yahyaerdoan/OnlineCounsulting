namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Tenancy's ModuleOfferingAdminResponse - flat-consumed, no ServerDataTable.</summary>
public record ModuleOfferingResponse(
    Guid Id,
    string Key,
    string Name,
    decimal Price,
    string BillingCycle,
    bool IsPubliclyVisible,
    string? ProviderProductId,
    string? ProviderPriceId);
