namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.FeatureHighlightsIntroModels;

/// <summary>Bound by FeatureHighlightsIntroFormPage for both create and edit.</summary>
public class FeatureHighlightsIntroFormModel
{
    public string Description { get; set; } = string.Empty;

    public Guid? CoverMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }
}
