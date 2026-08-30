using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>SiteContent admin nav - one entry per marketing-content list page under /admin/site-content.</summary>
public class SiteContentUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Site Content", Icons.Material.Outlined.Article, Color.Secondary,
        [
            new NavItem("About Us", $"{AdminPrefix}/site-content/about-us", Icons.Material.Outlined.Info),
            new NavItem("Footer Info", $"{AdminPrefix}/site-content/footer-info", Icons.Material.Outlined.SpaceDashboard),
            new NavItem("Gallery Categories", $"{AdminPrefix}/site-content/gallery-categories", Icons.Material.Outlined.Category),
            new NavItem("Gallery Items", $"{AdminPrefix}/site-content/gallery-items", Icons.Material.Outlined.PhotoLibrary),
            new NavItem("Service Process Steps", $"{AdminPrefix}/site-content/service-process-steps", Icons.Material.Outlined.Timeline),
            new NavItem("Service Offerings", $"{AdminPrefix}/site-content/service-offerings", Icons.Material.Outlined.Handyman),
            new NavItem("Service Areas", $"{AdminPrefix}/site-content/service-areas", Icons.Material.Outlined.Map),
            new NavItem("Hero Slides", $"{AdminPrefix}/site-content/hero-slides", Icons.Material.Outlined.ViewCarousel),
            new NavItem("Social Links", $"{AdminPrefix}/site-content/social-links", Icons.Material.Outlined.Share),
            new NavItem("Testimonials", $"{AdminPrefix}/site-content/testimonials", Icons.Material.Outlined.RateReview),
            new NavItem("Feature Highlights", $"{AdminPrefix}/site-content/feature-highlights", Icons.Material.Outlined.Stars),
            new NavItem("Partnerships", $"{AdminPrefix}/site-content/partnerships", Icons.Material.Outlined.Handshake),
            new NavItem("FAQ Items", $"{AdminPrefix}/site-content/faq-items", Icons.Material.Outlined.QuestionAnswer),
            new NavItem("Page Banners", $"{AdminPrefix}/site-content/page-banners", Icons.Material.Outlined.ViewDay),
        ]),
    ];
}
