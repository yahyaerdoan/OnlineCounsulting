namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.GalleryItemModels;

/// <summary>Bound by GalleryItemFormPage.</summary>
public class GalleryItemFormModel
{
    public string Description { get; set; } = string.Empty;

    public List<Guid> CategoryIds { get; set; } = [];

    public Guid? PhotoMediaAssetId { get; set; }

    public int DisplayOrder { get; set; }
}
