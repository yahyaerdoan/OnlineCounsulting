namespace OnlineConsulting.Maui.Shared.Pages.Admin.Catalog.CategoryModels;

/// <summary>Shared by CreateCategoryDialog and EditCategoryDialog.</summary>
public class CategoryFormModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
}
