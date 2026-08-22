using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;

public record ModuleOfferingListItemViewModel(
    Guid Id, string Key, string Name, decimal Price, string BillingCycle, bool IsPubliclyVisible);

public class CreateModuleOfferingViewModel
{
    [Required, MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string BillingCycle { get; set; } = "Monthly";

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public bool IsPubliclyVisible { get; set; } = true;
}

public class UpdateModuleOfferingViewModel
{
    public Guid Id { get; set; }

    /// <summary>Read-only reference fields - the Api's UpdateModuleOfferingCommand cannot change these
    /// (a real key/price change requires creating a new offering, per the Api's own doc comment).</summary>
    public string Key { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string BillingCycle { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsPubliclyVisible { get; set; } = true;
}
