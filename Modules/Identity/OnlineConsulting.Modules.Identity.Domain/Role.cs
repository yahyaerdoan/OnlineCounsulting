using Core.SecurityLayer.Identity;

namespace OnlineConsulting.Modules.Identity.Domain;

public class Role : SequentialGuidIdentityRole
{
    public string? Description { get; set; }
}
