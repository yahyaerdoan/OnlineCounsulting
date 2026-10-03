using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Marketing;

public sealed record ServiceNeed(string Title, string Why, IReadOnlyList<string> Keywords, bool SuggestMembership = false);

public sealed record ServiceFinderQuestion(string Id, string Text, ServiceFinderNode Yes, ServiceFinderNode No);

/// <summary>Either the next question or the recommended need; exactly one of the two is set.</summary>
public sealed record ServiceFinderNode(ServiceFinderQuestion? Question, ServiceNeed? Need)
{
    public static implicit operator ServiceFinderNode(ServiceFinderQuestion question) => new(question, null);

    public static implicit operator ServiceFinderNode(ServiceNeed need) => new(null, need);
}

/// <summary>Yes/no questions that map a homeowner's symptoms to a kind of service; services are matched by keyword so admin-entered catalog data works without code changes.</summary>
public static class ServiceFinderQuiz
{
    private static readonly ServiceNeed Repair = new(
        "Repair and diagnosis",
        "Something is not working as it should. A technician finds the cause first, so you only pay for the fix you need.",
        ["repair", "diagnos", "emergency", "fix"]);

    private static readonly ServiceNeed DuctCleaning = new(
        "Duct cleaning",
        "Dust, allergies and stale odors usually come from dirty ducts. Cleaning them improves air quality and airflow.",
        ["duct", "clean", "air quality", "vent"]);

    private static readonly ServiceNeed Installation = new(
        "Installation or replacement",
        "Older systems lose efficiency and parts get harder to find. A new or right-sized system can lower your bills.",
        ["install", "replace", "new system", "upgrade"]);

    private static readonly ServiceNeed Maintenance = new(
        "Maintenance and inspection",
        "Everything works, so the best move is keeping it that way. Regular maintenance prevents breakdowns and keeps efficiency up.",
        ["maintenance", "tune", "inspection", "check"],
        SuggestMembership: true);

    public static ServiceFinderQuestion Root { get; } = new(
        "not-working",
        "Is your heating or cooling not working properly, or making strange noises or smells?",
        Repair,
        new ServiceFinderQuestion(
            "air-quality",
            "Do you notice dust, allergies or musty odors, or have your ducts gone 3+ years without cleaning?",
            DuctCleaning,
            new ServiceFinderQuestion(
                "age",
                "Is your system older than about 12 years, or are you adding or renovating a room?",
                Installation,
                Maintenance)));

    public static IReadOnlyList<ServiceResponse> Match(ServiceNeed need, IEnumerable<ServiceResponse> services) =>
        [.. services.Where(service => need.Keywords.Any(keyword =>
            service.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            || service.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))];
}
