namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors the public GET /api/tenancy/module-offerings shape; distinct from ModuleOfferingResponse, the admin shape used by Pages/Admin/Platform/ModuleOfferings.razor.</summary>
public record PublicModuleOfferingResponse(string Key, string Name, decimal Price, string BillingCycle);
