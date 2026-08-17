using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Breadcrumb;

public record BreadcrumbListItemViewModel(Guid Id, string Title, string Description, string ImageUrl);

public class CreateBreadcrumbViewModel
{
    [Required, MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}

public class UpdateBreadcrumbViewModel : CreateBreadcrumbViewModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
}
