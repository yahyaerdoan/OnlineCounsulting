using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

public record HowIGetServiceListItemViewModel(Guid Id, string Title, string Description, string Icon, string? IconColor);

/// <summary>Icon is a plain class-name text input, not a dropdown - matches ServiceProcessStep's inline Icon + IconColor pattern.</summary>
public class CreateHowIGetServiceViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Icon { get; set; } = string.Empty;

    public string? IconColor { get; set; }
}

public class UpdateHowIGetServiceViewModel : CreateHowIGetServiceViewModel
{
    public Guid Id { get; set; }
}
