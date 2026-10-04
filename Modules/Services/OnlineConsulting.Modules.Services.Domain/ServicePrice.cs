namespace OnlineConsulting.Modules.Services.Domain;

/// <summary>A service's pricing as entered by staff; <see cref="Service.ChangePricing"/> checks it and derives the discounted price.</summary>
public sealed record ServicePrice(decimal Price, string PriceType, decimal? PriceMax, int DiscountRate, int TaxRate);
