namespace OnlineConsulting.Modules.SiteContent.Domain.Landing;

/// <summary>The landing list sections a LandingBlock can belong to.</summary>
public static class LandingSections
{
    public const string Feature = "Feature";
    public const string Step = "Step";
    public const string Faq = "Faq";

    public static readonly IReadOnlySet<string> All = new HashSet<string>([Feature, Step, Faq]);
}
