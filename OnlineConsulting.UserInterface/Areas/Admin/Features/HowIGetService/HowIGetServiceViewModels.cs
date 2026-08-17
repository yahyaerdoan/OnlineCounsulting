using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

public record HowIGetServiceListItemViewModel(Guid Id, string Title, string Description, string Icon, string? IconColor);

/// <summary>The legacy entity picked an icon via an ImgIcon foreign key - ServiceProcessStep (the new Api's
/// name for this concept) uses an inline Icon class name string + optional IconColor instead (same pattern
/// Categories already switched to), so this is a plain text input, not a dropdown.</summary>
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
