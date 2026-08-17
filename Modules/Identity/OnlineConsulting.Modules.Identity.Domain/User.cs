using Core.PersistenceLayer.Repositories.Entities;
using Core.SecurityLayer.Identity;

namespace OnlineConsulting.Modules.Identity.Domain;

public class User : SequentialGuidIdentityUser, IEntityAuditor
{
    public required Guid TenantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
}
