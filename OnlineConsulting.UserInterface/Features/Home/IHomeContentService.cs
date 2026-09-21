using OnlineConsulting.UserInterface.Features.Category;

namespace OnlineConsulting.UserInterface.Features.Home;

/// <summary>Api orchestration for the two home-page widgets without their own dedicated service (Categories, featured Services) - the rest have their own, e.g. ISliderItemService.</summary>
public interface IHomeContentService
{
    Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<HomeFeaturedServiceViewModel>> GetFeaturedServicesAsync(CancellationToken cancellationToken = default);
}
