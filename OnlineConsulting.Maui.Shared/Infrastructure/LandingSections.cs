namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>Mirrors the API's LandingSections values, with admin-facing labels.</summary>
public static class LandingSections
{
    public const string Feature = "Feature";
    public const string Step = "Step";
    public const string Faq = "Faq";

    public static readonly IReadOnlyList<(string Value, string Label)> All =
    [
        (Feature, "Feature card"),
        (Step, "How-it-works step"),
        (Faq, "FAQ entry"),
    ];

    public static string Label(string value) => All.FirstOrDefault(s => s.Value == value).Label ?? value;
}
