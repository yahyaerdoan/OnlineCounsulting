using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ServiceArea;

public record ServiceAreaListItemViewModel(Guid Id, string Name, string State, string Slug, string? IntroText, int DisplayOrder);

public class CreateServiceAreaViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string State { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? IntroText { get; set; }

    public int DisplayOrder { get; set; }
}

public class UpdateServiceAreaViewModel : CreateServiceAreaViewModel
{
    public Guid Id { get; set; }

    public string? Slug { get; set; }
}
