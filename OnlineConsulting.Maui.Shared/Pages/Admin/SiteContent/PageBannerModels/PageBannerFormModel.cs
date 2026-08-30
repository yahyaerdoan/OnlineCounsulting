namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.PageBannerModels;

/// <summary>Bound by PageBannerFormPage for both create and edit.</summary>
public class PageBannerFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
