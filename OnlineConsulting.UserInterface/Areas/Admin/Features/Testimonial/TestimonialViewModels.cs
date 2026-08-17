using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Testimonial;

public record TestimonialListItemViewModel(Guid Id, string FirstName, string LastName, string Title, string Description, string ImageUrl);

public class CreateTestimonialViewModel
{
    [Required, MinLength(1)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string LastName { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}

public class UpdateTestimonialViewModel : CreateTestimonialViewModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
}
