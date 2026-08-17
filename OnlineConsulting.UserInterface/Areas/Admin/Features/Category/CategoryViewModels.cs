using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Category;

public record CategoryListItemViewModel(Guid Id, string Title, string Description, string Icon, string? IconColor);

/// <summary>The legacy entity picked its icon through an ImgIcon foreign key - the Categories module dropped
/// that column in favour of an inline Icon class name + optional IconColor, so this is a plain text input
/// (same pattern as ServiceProcessStep/ServiceOffering), not a dropdown and not a file upload.</summary>
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
