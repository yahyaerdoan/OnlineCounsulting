namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.FooterInfoModels;

/// <summary>Shared by FooterInfoFormPage (create and edit).</summary>
public class FooterInfoFormModel
{
    public string ImageUrl { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
