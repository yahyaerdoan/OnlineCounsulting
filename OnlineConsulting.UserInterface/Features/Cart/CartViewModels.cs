namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>ServiceTitle is resolved server-side (CartItemResponse only carries the plain ServiceId).</summary>
public record CartLineViewModel(Guid Id, string ServiceTitle, int Quantity, decimal Price, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

public record CartViewModel(Guid CartId, List<CartLineViewModel> Items, decimal SubTotal, decimal Total);
