using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Pages.Admin.Catalog.ServiceModels;

/// <summary>Shared by CreateServiceDialog and EditServiceDialog.</summary>
public class ServiceFormModel
{
    public Guid CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string DetailedDescription { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string PriceType { get; set; } = ServicePriceTypes.Fixed;

    public decimal? PriceMax { get; set; }

    public bool FeaturedArea { get; set; }

    public int DiscountRate { get; set; }

    public int TaxRate { get; set; }

    public bool RequiresPrepayment { get; set; }

    public bool IsEmergencyAvailable { get; set; }

    public Guid? CoverMediaAssetId { get; set; }
}
