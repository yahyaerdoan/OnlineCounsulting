using Hateoas;
using OnlineConsulting.Modules.Categories.Domain;

namespace OnlineConsulting.Modules.Categories.Application.Contracts;

// Records can't inherit a plain class (LinkedResponse) - so a class with required init properties instead of a positional record.
public class CategoryResponse : LinkedResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid ImgIconId { get; init; }

    public static CategoryResponse FromDomain(Category category) => new()
    {
        Id = category.Id,
        Title = category.Title,
        Description = category.Description,
        ImgIconId = category.ImgIconId,
    };
}
