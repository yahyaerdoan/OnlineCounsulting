using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Categories.Domain;

public class Category : TenantEntity<Guid>
{
    public required string Title { get; set; }
    public required string Description { get; set; }

    /// <summary>
    /// Plain id, no navigation - ImgIcon still lives in the legacy shared DbContext, and
    /// modules never reference each other's entities directly, only by id.
    /// </summary>
    public required Guid ImgIconId { get; set; }
}
