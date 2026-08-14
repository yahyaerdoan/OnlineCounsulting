using Core.SecurityLayer.Identity;

namespace OnlineConsulting.Modules.Identity.Domain;

public class User : SequentialGuidIdentityUser
{
    public required Guid TenantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool IsActive { get; set; }
}
