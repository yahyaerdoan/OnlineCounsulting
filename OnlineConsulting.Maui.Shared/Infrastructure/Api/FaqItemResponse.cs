namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors POST /api/site-content/faq-items/query's response shape.</summary>
public record FaqItemResponse(Guid Id, Guid ServiceId, string Question, string Answer, int DisplayOrder) : IQueryableFields
{
    public static string[] SearchFields => [nameof(Question), nameof(Answer)];
}
