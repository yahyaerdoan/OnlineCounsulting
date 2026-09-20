namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Mirrors GET /api/basket's response shape.</summary>
public record BasketResponse(Guid Id, int Quantity, decimal SubTotalPrice, decimal TotalPrice, IReadOnlyList<BasketItemResponse> Items);

/// <summary>One basket line - Id is the basket item id (pass to RemoveItem), not the ServiceId.</summary>
public record BasketItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal Price, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);
