namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.FaqItemModels;

public class FaqItemFormModel
{
    public Guid ServiceId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
