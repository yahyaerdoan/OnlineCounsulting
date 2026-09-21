using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

public record ProvidedItemListItemViewModel(Guid Id, string Title, string Description, string Icon, string? IconColor);

/// <summary>Icon is a plain class-name text input, not a dropdown - matches ServiceOffering's inline Icon + IconColor pattern.</summary>
public class CreateProvidedItemViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
}

public class UpdateProvidedItemViewModel : CreateProvidedItemViewModel
{
    public Guid Id { get; set; }
}
