using MudBlazor;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>SiteContent admin nav - one entry per marketing-content list page under /admin/site-content.</summary>
public class SiteContentUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Site Content", Icons.Material.Outlined.Article, Color.Secondary,
        [
            new NavItem("About Us", $"{AdminPrefix}/site-content/about-us", Icons.Material.Outlined.Info, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Footer Info", $"{AdminPrefix}/site-content/footer-info", Icons.Material.Outlined.SpaceDashboard, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Gallery Categories", $"{AdminPrefix}/site-content/gallery-categories", Icons.Material.Outlined.Category, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Gallery Items", $"{AdminPrefix}/site-content/gallery-items", Icons.Material.Outlined.PhotoLibrary, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Service Process Steps", $"{AdminPrefix}/site-content/service-process-steps", Icons.Material.Outlined.Timeline, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Service Offerings", $"{AdminPrefix}/site-content/service-offerings", Icons.Material.Outlined.Handyman, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Service Areas", $"{AdminPrefix}/site-content/service-areas", Icons.Material.Outlined.Map, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Hero Slides", $"{AdminPrefix}/site-content/hero-slides", Icons.Material.Outlined.ViewCarousel, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Social Links", $"{AdminPrefix}/site-content/social-links", Icons.Material.Outlined.Share, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Testimonials", $"{AdminPrefix}/site-content/testimonials", Icons.Material.Outlined.RateReview, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Feature Highlights", $"{AdminPrefix}/site-content/feature-highlights", Icons.Material.Outlined.Stars, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Feature Highlights Intro", $"{AdminPrefix}/site-content/feature-highlights-intro", Icons.Material.Outlined.Notes, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Partnerships", $"{AdminPrefix}/site-content/partnerships", Icons.Material.Outlined.Handshake, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("FAQ Items", $"{AdminPrefix}/site-content/faq-items", Icons.Material.Outlined.QuestionAnswer, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Page Banners", $"{AdminPrefix}/site-content/page-banners", Icons.Material.Outlined.ViewDay, RequiredPermission: PermissionAreas.SiteContent),
            new NavItem("Policies", $"{AdminPrefix}/site-content/legal-policies", Icons.Material.Outlined.Gavel, RequiredPermission: PermissionAreas.SiteContent),
        ]),
    ];
}
