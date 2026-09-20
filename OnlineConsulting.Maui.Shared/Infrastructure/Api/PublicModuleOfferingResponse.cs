namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/tenancy/module-offerings's response shape - the public catalog for tenant
/// self-service signup. Distinct from ModuleOfferingResponse, which mirrors the admin shape (has Id/
/// IsPubliclyVisible/provider ids) used by Pages/Admin/Platform/ModuleOfferings.razor.</summary>
public record PublicModuleOfferingResponse(string Key, string Name, decimal Price, string BillingCycle);
