using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Catalog;

namespace OnlineConsulting.Modules.Services.Tests;

public class ServiceTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    private static Service Create(ServicePrice price) =>
        Service.Create(CategoryId, "Duct cleaning", "duct-cleaning", "Whole-house duct cleaning", "A long detailed description of the work.", ServiceKinds.Booking, price);

    [Fact]
    public void Create_DerivesDiscountedPriceRoundedToCents()
    {
        var service = Create(new ServicePrice(99.99m, ServicePriceTypes.Fixed, null, 15, 8));

        Assert.Equal(84.99m, service.DiscountedPrice);
        Assert.Equal(99.99m, service.Price);
        Assert.Equal(15, service.DiscountRate);
        Assert.Equal(8, service.TaxRate);
    }

    [Fact]
    public void ChangePricing_KeepsDiscountedPriceInStep()
    {
        var service = Create(new ServicePrice(100m, ServicePriceTypes.Fixed, null, 0, 0));

        service.ChangePricing(new ServicePrice(200m, ServicePriceTypes.Range, 300m, 10, 0));

        Assert.Equal(180m, service.DiscountedPrice);
        Assert.Equal(ServicePriceTypes.Range, service.PriceType);
        Assert.Equal(300m, service.PriceMax);
    }

    [Theory]
    [InlineData(0, "Fixed", null, 0, 0)]
    [InlineData(100, "Fixed", null, -1, 0)]
    [InlineData(100, "Fixed", null, 101, 0)]
    [InlineData(100, "Fixed", null, 0, 101)]
    [InlineData(100, "Hourly", null, 0, 0)]
    [InlineData(100, "Range", null, 0, 0)]
    [InlineData(100, "Range", 100, 0, 0)]
    [InlineData(100, "Fixed", 200, 0, 0)]
    public void ChangePricing_InvalidPricing_ThrowsAndKeepsOldPricing(int price, string priceType, int? priceMax, int discountRate, int taxRate)
    {
        var service = Create(new ServicePrice(50m, ServicePriceTypes.Fixed, null, 10, 0));

        _ = Assert.ThrowsAny<ArgumentException>(() => service.ChangePricing(new ServicePrice(price, priceType, priceMax, discountRate, taxRate)));
        Assert.Equal(50m, service.Price);
        Assert.Equal(45m, service.DiscountedPrice);
    }

    [Fact]
    public void UpdateDetails_RejectsUnknownKindAndMissingCategory()
    {
        var service = Create(new ServicePrice(50m, ServicePriceTypes.Fixed, null, 0, 0));

        _ = Assert.Throws<ArgumentException>(() => service.UpdateDetails(CategoryId, "Title", "Desc", "Details", "Subscription"));
        _ = Assert.Throws<ArgumentException>(() => service.UpdateDetails(Guid.Empty, "Title", "Desc", "Details", ServiceKinds.Product));
    }

    [Fact]
    public void SetOptions_SetsFlagsAndCover()
    {
        var service = Create(new ServicePrice(50m, ServicePriceTypes.Fixed, null, 0, 0));
        var cover = Guid.NewGuid();

        service.SetOptions(featuredArea: true, requiresPrepayment: true, isEmergencyAvailable: true, cover);

        Assert.True(service.FeaturedArea);
        Assert.True(service.RequiresPrepayment);
        Assert.True(service.IsEmergencyAvailable);
        Assert.Equal(cover, service.CoverMediaAssetId);
    }
}
