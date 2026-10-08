using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Tests.Baskets;

public class BasketTests
{
    private static readonly Guid FilterId = Guid.NewGuid();
    private static readonly Guid ThermostatId = Guid.NewGuid();

    private static Basket UserBasket() => Basket.Open(Guid.NewGuid(), null);

    private static BasketItem Saved(BasketItem item)
    {
        item.Id = Guid.NewGuid();
        return item;
    }

    [Fact]
    public void Open_NeedsExactlyOneOwner()
    {
        _ = Assert.Throws<ArgumentException>(() => Basket.Open(null, null));
        _ = Assert.Throws<ArgumentException>(() => Basket.Open(Guid.NewGuid(), Guid.NewGuid()));

        var guestBasket = Basket.Open(null, Guid.NewGuid());
        Assert.True(guestBasket.IsEmpty);
        Assert.Null(guestBasket.UserId);
    }

    [Fact]
    public void AddItem_NewService_AddsLineAndTotals()
    {
        var basket = UserBasket();

        basket.AddItem(FilterId, 2, 10m, 8);

        var item = Assert.Single(basket.Items);
        Assert.Equal(basket.Id, item.BasketId);
        Assert.Equal(20m, item.SubTotalPrice);
        Assert.Equal(1.6m, item.TaxAmount);
        Assert.Equal(21.6m, item.TotalPrice);
        Assert.Equal(2, basket.Quantity);
        Assert.Equal(20m, basket.SubTotalPrice);
        Assert.Equal(21.6m, basket.TotalPrice);
    }

    [Fact]
    public void AddItem_SameService_GrowsLineAndTakesNewPrice()
    {
        var basket = UserBasket();
        basket.AddItem(FilterId, 1, 10m, 0);

        basket.AddItem(FilterId, 2, 12m, 0);

        var item = Assert.Single(basket.Items);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(12m, item.Price);
        Assert.Equal(36m, basket.TotalPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithNonPositiveQuantity_Throws(int quantity) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => UserBasket().AddItem(FilterId, quantity, 10m, 0));

    [Fact]
    public void SetItemQuantity_UpdatesLineAndTotals()
    {
        var basket = UserBasket();
        basket.AddItem(FilterId, 1, 10m, 0);
        basket.AddItem(ThermostatId, 1, 100m, 0);
        var filter = Saved(basket.Items[0]);

        basket.SetItemQuantity(filter.Id, 4);

        Assert.Equal(4, filter.Quantity);
        Assert.Equal(5, basket.Quantity);
        Assert.Equal(140m, basket.TotalPrice);
    }

    [Fact]
    public void SetItemQuantity_UnknownItem_Throws() =>
        Assert.Throws<InvalidOperationException>(() => UserBasket().SetItemQuantity(Guid.NewGuid(), 1));

    [Fact]
    public void RemoveItem_DropsLineAndTotals()
    {
        var basket = UserBasket();
        basket.AddItem(FilterId, 1, 10m, 0);
        basket.AddItem(ThermostatId, 1, 100m, 0);
        var thermostat = Saved(basket.Items[1]);

        basket.RemoveItem(thermostat.Id);

        Assert.Single(basket.Items);
        Assert.Null(basket.FindItem(thermostat.Id));
        Assert.Equal(10m, basket.TotalPrice);
    }

    [Fact]
    public void Clear_EmptiesBasketAndZeroesTotals()
    {
        var basket = UserBasket();
        basket.AddItem(FilterId, 3, 10m, 5);

        basket.Clear();

        Assert.True(basket.IsEmpty);
        Assert.Equal(0, basket.Quantity);
        Assert.Equal(0m, basket.TotalPrice);
    }

    [Fact]
    public void Reprice_ChangesOnlyThatServiceLine()
    {
        var basket = UserBasket();
        basket.AddItem(FilterId, 2, 10m, 0);
        basket.AddItem(ThermostatId, 1, 100m, 0);

        basket.Reprice(FilterId, 15m, 10);

        Assert.Equal(15m, basket.Items[0].Price);
        Assert.Equal(33m, basket.Items[0].TotalPrice);
        Assert.Equal(100m, basket.Items[1].Price);
        Assert.Equal(133m, basket.TotalPrice);
    }

    [Fact]
    public void MergeFrom_AddsQuantitiesKeepsOwnPriceAndEmptiesGuestBasket()
    {
        var userBasket = UserBasket();
        userBasket.AddItem(FilterId, 1, 10m, 0);
        var guestBasket = Basket.Open(null, Guid.NewGuid());
        guestBasket.AddItem(FilterId, 2, 9m, 0);
        guestBasket.AddItem(ThermostatId, 1, 100m, 0);

        userBasket.MergeFrom(guestBasket);

        Assert.Equal(2, userBasket.Items.Count);
        Assert.Equal(3, userBasket.Items[0].Quantity);
        Assert.Equal(10m, userBasket.Items[0].Price);
        Assert.Equal(userBasket.Id, userBasket.Items[1].BasketId);
        Assert.Equal(130m, userBasket.TotalPrice);
        Assert.True(guestBasket.IsEmpty);
        Assert.Equal(0m, guestBasket.TotalPrice);
    }

    [Fact]
    public void MergeFrom_Itself_Throws()
    {
        var basket = UserBasket();

        _ = Assert.Throws<InvalidOperationException>(() => basket.MergeFrom(basket));
    }
}
