using Microsoft.AspNetCore.Identity;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Role : IdentityRole<string>
{
    public string? Description { get; set; }
}
