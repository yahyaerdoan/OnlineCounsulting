using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.SiteContent.Domain.Service;

public class ServiceArea : SequentialGuidTenantEntity
{
    public required string Name { get; set; }
    public required string State { get; set; }
    public required string Slug { get; set; }
    public string? IntroText { get; set; }
    public int DisplayOrder { get; set; }

    /// <summary>City center for the map pin - looked up from Name and State when the area is saved; null until found.</summary>
    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}
