namespace OnlineConsulting.Maui.Shared.Pages.Admin.Platform.BundleModels;

public class BundleFormModel
{
    public string Name { get; set; } = string.Empty;
    public List<string> ModuleKeys { get; set; } = [];
    public bool IsPubliclyVisible { get; set; }
}
