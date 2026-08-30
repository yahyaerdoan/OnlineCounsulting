namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/service-process-steps/query's response shape.</summary>
public record ServiceProcessStepResponse(Guid Id, string Title, string Description, string Icon, string? IconColor, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Title), nameof(Description)];
}
