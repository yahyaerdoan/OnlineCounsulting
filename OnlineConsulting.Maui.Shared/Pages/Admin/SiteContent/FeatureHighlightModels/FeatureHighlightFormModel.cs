namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.FeatureHighlightModels;

/// <summary>Bound by FeatureHighlightFormPage for both create and edit.</summary>
public class FeatureHighlightFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
