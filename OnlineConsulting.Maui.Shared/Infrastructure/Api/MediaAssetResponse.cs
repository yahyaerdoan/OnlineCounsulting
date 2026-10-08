
namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/v1/media/{id}'s response shape.</summary>
public record MediaAssetResponse(Guid Id, string Url, string? AltText, string ContentType, long SizeBytes, int? Width, int? Height);
