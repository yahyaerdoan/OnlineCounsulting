namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/service-areas/query's response shape.</summary>
public record ServiceAreaResponse(Guid Id, string Name, string State, string Slug, string? IntroText, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Name), nameof(State)];
}
