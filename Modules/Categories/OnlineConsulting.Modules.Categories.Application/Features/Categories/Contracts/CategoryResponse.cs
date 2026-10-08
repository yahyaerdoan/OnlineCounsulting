using OnlineConsulting.Modules.Categories.Domain;

namespace OnlineConsulting.Modules.Categories.Application.Features.Categories.Contracts;

public class CategoryResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Icon { get; init; }
    public string? IconColor { get; init; }

    public static CategoryResponse FromDomain(Category category) => new()
    {
        Id = category.Id,
        Title = category.Title,
        Description = category.Description,
        Icon = category.Icon,
        IconColor = category.IconColor,
    };
}
