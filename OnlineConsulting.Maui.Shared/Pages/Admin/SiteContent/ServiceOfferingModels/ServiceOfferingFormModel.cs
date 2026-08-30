namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.ServiceOfferingModels;

/// <summary>Bound by ServiceOfferingFormPage for both create and edit.</summary>
public class ServiceOfferingFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }

    public int DisplayOrder { get; set; }
}
