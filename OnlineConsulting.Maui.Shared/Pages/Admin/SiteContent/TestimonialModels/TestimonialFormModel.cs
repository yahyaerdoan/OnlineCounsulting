namespace OnlineConsulting.Maui.Shared.Pages.Admin.SiteContent.TestimonialModels;

/// <summary>Shared by CreateTestimonialDialog and EditTestimonialDialog.</summary>
public class TestimonialFormModel
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
