using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Domain;

public class Category : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    /// <summary>MudBlazor icon value (e.g. Icons.Material.Filled.Category) - a plain portable string, not a reference to any template's bundled icon font. Replaces the old legacy-DbContext-coupled ImgIconId.</summary>
    public required string Icon { get; set; }

    /// <summary>Hex color (e.g. "#FF5733") applied to Icon. Null means the frontend uses its default theme color (currentColor).</summary>
    public string? IconColor { get; set; }
}
