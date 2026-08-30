namespace OnlineConsulting.Maui.Shared.Pages.Admin.Inquiries.ContactModels;

public class ContactFormModel
{
    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string WorkingHours { get; set; } = string.Empty;
}

/// <summary>Local mirror of the Api-side CompanyContactResponse - Maui.Shared can't reference the Application project.</summary>
public record CompanyContactResponse(Guid Id, string Email, string Phone, string Address, string Description, string WorkingHours);
