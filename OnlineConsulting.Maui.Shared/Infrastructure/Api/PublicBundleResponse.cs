namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/tenancy/bundles's response shape - the public catalog for tenant self-service
/// signup. Distinct from BundleResponse, which mirrors the admin shape (has Id/IsPubliclyVisible) used by
/// Pages/Admin/Platform/Bundles.razor.</summary>
public record PublicBundleResponse(string Name, List<string> ModuleKeys);
