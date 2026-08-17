using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;

/// <summary>The Api's /api/contact is a singleton upsert (GetContact/UpdateContact only, no per-id Create/Delete)
/// - there is exactly one CompanyContact row per tenant, so this view model carries no Id.</summary>
public class ContactViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string WorkingHours { get; set; } = string.Empty;

    [Required, MinLength(5)]
    public string Description { get; set; } = string.Empty;
}
