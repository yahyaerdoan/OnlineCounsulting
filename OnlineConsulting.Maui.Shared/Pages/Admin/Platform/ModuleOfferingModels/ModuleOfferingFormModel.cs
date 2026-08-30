namespace OnlineConsulting.Maui.Shared.Pages.Admin.Platform.ModuleOfferingModels;

public class ModuleOfferingFormModel
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = "Monthly";
    public bool IsPubliclyVisible { get; set; }
}
