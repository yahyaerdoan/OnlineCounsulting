namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.FaqItemModels;

public class FaqItemFormModel
{
    /// <summary>Null = general FAQ.</summary>
    public Guid? ServiceId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
