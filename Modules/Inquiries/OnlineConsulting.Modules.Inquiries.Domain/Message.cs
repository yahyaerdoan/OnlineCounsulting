using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Domain;

public class Message : SequentialGuidTenantEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Subject { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset? RepliedAt { get; set; }
}
