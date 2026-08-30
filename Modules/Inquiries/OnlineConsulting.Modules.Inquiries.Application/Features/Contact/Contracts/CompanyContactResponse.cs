using Hateoas;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;

/// <summary>Contact response as a class with required init properties, since records can't inherit the plain LinkedResponse class.</summary>
public class CompanyContactResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required string Address { get; init; }
    public required string Description { get; init; }
    public required string WorkingHours { get; init; }

    public static CompanyContactResponse FromDomain(CompanyContact contact) => new()
    {
        Id = contact.Id,
        Email = contact.Email,
        Phone = contact.Phone,
        Address = contact.Address,
        Description = contact.Description,
        WorkingHours = contact.WorkingHours,
    };
}
