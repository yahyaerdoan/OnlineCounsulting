using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;

public record CompanyContactResponse(Guid Id, string Email, string Phone, string Address, string Description, string WorkingHours)
{
    public static CompanyContactResponse FromDomain(CompanyContact contact) =>
        new(contact.Id, contact.Email, contact.Phone, contact.Address, contact.Description, contact.WorkingHours);
}
