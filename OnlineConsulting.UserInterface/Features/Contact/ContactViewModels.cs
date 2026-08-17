using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Features.Contact;

/// <summary>Bound from the public contact-form partial (ContactMessageComponentPartial) - submitting this
/// creates an Inquiries.Message on the Api side, "Contact" is the page/route name, not the Api resource.</summary>
public class CreateMessageViewModel
{
    [Required, MinLength(1)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public string Subject { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;
}
