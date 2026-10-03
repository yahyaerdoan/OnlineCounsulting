namespace OnlineConsulting.Maui.Shared.Theme;

/// <summary>Accent tone CSS classes (app.css "tone-*") for colorful icon bubbles; At() rotates through them for data-driven lists.</summary>
public static class AccentTone
{
    public const string Blue = "tone-blue";
    public const string Green = "tone-green";
    public const string Orange = "tone-orange";
    public const string Red = "tone-red";
    public const string Teal = "tone-teal";
    public const string Gold = "tone-gold";
    public const string Berry = "tone-berry";
    public const string Cornflower = "tone-cornflower";

    private static readonly string[] Rotation = [Blue, Green, Orange, Teal, Berry, Gold, Cornflower, Red];

    public static string At(int index) => Rotation[Math.Abs(index) % Rotation.Length];

    /// <summary>A stable tone per id (e.g. a category), so the same category keeps its color on every page.</summary>
    public static string For(Guid id) => At(id.GetHashCode());
}
