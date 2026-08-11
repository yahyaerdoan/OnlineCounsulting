using Microsoft.AspNetCore.Identity;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class User : IdentityUser<string>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpirationDue { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? CreatedDate { get; set; }
    public virtual string? CreatedBy { get; set; }
    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string? UpdatedBy { get; set; }
    public virtual string? DeletedBy { get; set; }
    public virtual DateTime? DeletedDate { get; set; }
    public bool Status { get; set; }

    public ICollection<Basket>? Basket { get; set; }
    public ICollection<UserAddress>? UserAddresses { get; set; }
    public ICollection<Order>? Orders { get; set; }
}
