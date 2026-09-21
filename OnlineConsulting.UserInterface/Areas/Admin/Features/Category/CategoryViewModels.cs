using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Category;

public record CategoryListItemViewModel(Guid Id, string Title, string Description, string Icon, string? IconColor);

/// <summary>Icon is a plain class-name text input (not a dropdown/upload) - the legacy ImgIcon FK was dropped in favor of inline Icon + IconColor.</summary>
public class CreateCategoryViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
}

public class UpdateCategoryViewModel : CreateCategoryViewModel
{
    public Guid Id { get; set; }
}
