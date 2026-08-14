using Hateoas;
using OnlineConsulting.Modules.Services.Domain;

namespace OnlineConsulting.Modules.Services.Application.Contracts;

// Records can't inherit a plain class (LinkedResponse) - so a class with required init properties instead of a positional record.
public class ServiceResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required string Description { get; init; }
    public required string DetailedDescription { get; init; }
    public required decimal Price { get; init; }
    public required bool FeaturedArea { get; init; }
    public required int DiscountRate { get; init; }
    public required int TaxRate { get; init; }
    public required decimal DiscountedPrice { get; init; }

    public static ServiceResponse FromDomain(Service service) => new()
    {
        Id = service.Id,
        CategoryId = service.CategoryId,
        Title = service.Title,
        Slug = service.Slug,
        Description = service.Description,
        DetailedDescription = service.DetailedDescription,
        Price = service.Price,
        FeaturedArea = service.FeaturedArea,
        DiscountRate = service.DiscountRate,
        TaxRate = service.TaxRate,
        DiscountedPrice = service.DiscountedPrice,
    };
}
