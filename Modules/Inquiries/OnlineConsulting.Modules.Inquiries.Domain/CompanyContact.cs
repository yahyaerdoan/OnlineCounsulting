using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

public class CompanyContact : TenantEntity<Guid>
{
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string Address { get; set; }
    public required string Description { get; set; }
    public required string WorkingHours { get; set; }
}
