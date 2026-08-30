namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.ServiceAreaModels;

/// <summary>Bound by ServiceAreaFormPage. No Slug - server-generated, never editable.</summary>
public class ServiceAreaFormModel
{
    public string Name { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string? IntroText { get; set; }

    public int DisplayOrder { get; set; }
}
