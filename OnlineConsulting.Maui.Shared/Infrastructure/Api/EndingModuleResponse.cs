namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors Tenancy's EndingModuleResponse - a removed module still usable until EndsAt (UTC).</summary>
public record EndingModuleResponse(string ModuleKey, DateTime EndsAt);
