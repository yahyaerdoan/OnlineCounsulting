namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/service-offerings/query's response shape.</summary>
public record ServiceOfferingResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
