using Core.PersistenceLayer.MultiTenancy;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Services.Domain;

/// <summary>A bookable service or a product in the catalog. Pricing changes only through <see cref="ChangePricing"/>, which keeps the discounted price in step.</summary>
public class Service : SequentialGuidTenantEntity
{
    private Service()
    {
    }

    /// <summary>Plain id, no navigation, since modules never reference each other's entities directly, only by id.</summary>
    public Guid CategoryId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string DetailedDescription { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    /// <summary>One of <see cref="ServicePriceTypes"/>.</summary>
    public string PriceType { get; private set; } = ServicePriceTypes.Fixed;

    /// <summary>Set only when <see cref="PriceType"/> is Range.</summary>
    public decimal? PriceMax { get; private set; }

    public bool FeaturedArea { get; private set; }
    public int DiscountRate { get; private set; }
    public int TaxRate { get; private set; }

    /// <summary><see cref="Price"/> less <see cref="DiscountRate"/>, rounded to cents; never supplied by a caller.</summary>
    public decimal DiscountedPrice { get; private set; }

    /// <summary>When true, a Scheduling appointment must reach PendingPayment/Confirmed via a paid Commerce order before the tenant confirms it.</summary>
    public bool RequiresPrepayment { get; private set; }

    /// <summary>Whether this service can be requested as an urgent/24-7 callout (e.g. "HVAC Emergency"), an urgency modifier on the same service.</summary>
    public bool IsEmergencyAvailable { get; private set; }

    /// <summary>ServiceKinds.* - Booking is scheduled through the appointment flow, Product is bought through basket and checkout.</summary>
    public string Kind { get; private set; } = ServiceKinds.Booking;

    /// <summary>Plain id, no navigation - MediaAsset lives in the Media module. Null means no cover photo uploaded yet.</summary>
    public Guid? CoverMediaAssetId { get; private set; }

    public static Service Create(Guid categoryId, string title, string slug, string description, string detailedDescription, string kind, ServicePrice price)
    {
        var service = new Service();
        service.UpdateDetails(categoryId, title, description, detailedDescription, kind);
        service.ChangeSlug(slug);
        service.ChangePricing(price);
        return service;
    }

    public void UpdateDetails(Guid categoryId, string title, string description, string detailedDescription, string kind)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("A service needs a category.", nameof(categoryId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(detailedDescription);

        if (!ServiceKinds.All.Contains(kind))
        {
            throw new ArgumentException($"Unknown service kind '{kind}'.", nameof(kind));
        }

        CategoryId = categoryId;
        Title = title;
        Description = description;
        DetailedDescription = detailedDescription;
        Kind = kind;
    }

    /// <summary>The URL slug; generate it unique within the tenant before calling.</summary>
    public void ChangeSlug(string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        Slug = slug;
    }

    /// <summary>Replaces the pricing: price above zero, rates 0-100, a maximum above the price only for Range. Recomputes <see cref="DiscountedPrice"/>.</summary>
    public void ChangePricing(ServicePrice price)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price.Price);
        ArgumentOutOfRangeException.ThrowIfNegative(price.DiscountRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(price.DiscountRate, 100);
        ArgumentOutOfRangeException.ThrowIfNegative(price.TaxRate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(price.TaxRate, 100);

        if (!ServicePriceTypes.All.Contains(price.PriceType))
        {
            throw new ArgumentException($"Unknown price type '{price.PriceType}'.", nameof(price));
        }

        if (price.PriceType == ServicePriceTypes.Range ? price.PriceMax is not { } max || max <= price.Price : price.PriceMax is not null)
        {
            throw new ArgumentException("A maximum price above the price is required for Range and not allowed otherwise.", nameof(price));
        }

        Price = price.Price;
        PriceType = price.PriceType;
        PriceMax = price.PriceMax;
        DiscountRate = price.DiscountRate;
        TaxRate = price.TaxRate;
        DiscountedPrice = Math.Round(price.Price - (price.Price * price.DiscountRate / 100m), 2, MidpointRounding.AwayFromZero);
    }

    public void SetOptions(bool featuredArea, bool requiresPrepayment, bool isEmergencyAvailable, Guid? coverMediaAssetId)
    {
        FeaturedArea = featuredArea;
        RequiresPrepayment = requiresPrepayment;
        IsEmergencyAvailable = isEmergencyAvailable;
        CoverMediaAssetId = coverMediaAssetId;
    }
}
