using Hateoas;
using OnlineConsulting.Modules.Services.Domain;

namespace OnlineConsulting.Modules.Services.Application.Contracts;

/// <summary>A class with required init properties instead of a positional record, since records can't inherit LinkedResponse.</summary>
public class ServiceResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required string Description { get; init; }
    public required string DetailedDescription { get; init; }
    public required decimal Price { get; init; }
    public required string PriceType { get; init; }
    public decimal? PriceMax { get; init; }
    public required bool FeaturedArea { get; init; }
    public required int DiscountRate { get; init; }
    public required int TaxRate { get; init; }
    public required decimal DiscountedPrice { get; init; }
    public required bool RequiresPrepayment { get; init; }
    public required bool IsEmergencyAvailable { get; init; }
    public Guid? CoverMediaAssetId { get; init; }

    /// <summary>The extended photo/video gallery - empty for list-view queries (GetServices, SearchServices, etc.) to avoid an N+1 join per row, populated only by GetServiceById/GetServiceBySlug where a single service's full detail is being fetched.</summary>
    public List<ServiceMediaItemResponse> MediaItems { get; init; } = [];

    public static ServiceResponse FromDomain(Service service, List<ServiceMediaItemResponse>? mediaItems = null) => new()
    {
        Id = service.Id,
        CategoryId = service.CategoryId,
        Title = service.Title,
        Slug = service.Slug,
        Description = service.Description,
        DetailedDescription = service.DetailedDescription,
        Price = service.Price,
        PriceType = service.PriceType,
        PriceMax = service.PriceMax,
        FeaturedArea = service.FeaturedArea,
        DiscountRate = service.DiscountRate,
        TaxRate = service.TaxRate,
        DiscountedPrice = service.DiscountedPrice,
        RequiresPrepayment = service.RequiresPrepayment,
        IsEmergencyAvailable = service.IsEmergencyAvailable,
        CoverMediaAssetId = service.CoverMediaAssetId,
        MediaItems = mediaItems ?? [],
    };
}
