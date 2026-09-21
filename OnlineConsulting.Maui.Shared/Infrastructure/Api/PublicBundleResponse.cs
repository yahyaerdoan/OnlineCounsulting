namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors the public GET /api/tenancy/bundles shape; distinct from BundleResponse, the admin shape (has Id/IsPubliclyVisible) used by Pages/Admin/Platform/Bundles.razor.</summary>
public record PublicBundleResponse(string Name, List<string> ModuleKeys);
