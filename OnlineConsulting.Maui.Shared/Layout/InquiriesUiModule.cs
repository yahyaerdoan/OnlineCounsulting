using MudBlazor;

namespace OnlineConsulting.Maui.Shared.Layout;

/// <summary>Contact/Messages/Newsletter admin nav - migrated from the legacy MVC Inquiries controllers.</summary>
public class InquiriesUiModule : IUiModule
{
    private const string AdminPrefix = AppRoutes.AdminHome;

    public IReadOnlyList<NavSection> NavSections { get; } =
    [
        new NavSection("Inquiries", Icons.Material.Outlined.MarkEmailUnread, Color.Info,
        [
            new NavItem("Contact Info", $"{AdminPrefix}/inquiries/contact", Icons.Material.Outlined.ContactMail),
            new NavItem("Messages", $"{AdminPrefix}/inquiries/messages", Icons.Material.Outlined.Message),
            new NavItem("Newsletter", $"{AdminPrefix}/inquiries/newsletter", Icons.Material.Outlined.Mail),
        ]),
    ];
}
