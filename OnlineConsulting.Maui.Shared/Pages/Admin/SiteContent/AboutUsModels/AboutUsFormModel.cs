namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.AboutUsModels;

/// <summary>Shared by CreateAboutUsDialog and EditAboutUsDialog.</summary>
public class AboutUsFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? CoverImage { get; set; }

    public string? VideoUrl { get; set; }

    public int DisplayOrder { get; set; }
}
