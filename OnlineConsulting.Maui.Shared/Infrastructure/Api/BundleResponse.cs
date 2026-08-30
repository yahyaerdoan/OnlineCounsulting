namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Tenancy's BundleAdminResponse - flat-consumed, no ServerDataTable.</summary>
public record BundleResponse(Guid Id, string Name, List<string> ModuleKeys, bool IsPubliclyVisible);
