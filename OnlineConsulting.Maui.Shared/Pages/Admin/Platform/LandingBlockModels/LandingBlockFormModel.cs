using OnlineConsulting.Maui.Shared.Infrastructure;

namespace OnlineConsulting.Maui.Shared.Pages.Admin.Platform.LandingBlockModels;

/// <summary>Bound by LandingBlockFormPage for both create and edit.</summary>
public class LandingBlockFormModel
{
    public string Section { get; set; } = LandingSections.Feature;

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }
}
