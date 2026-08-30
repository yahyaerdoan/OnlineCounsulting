namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.SocialLinkModels;

/// <summary>Bound by SocialLinkFormPage for both create and edit.</summary>
public class SocialLinkFormModel
{
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }

    public int DisplayOrder { get; set; }
}
