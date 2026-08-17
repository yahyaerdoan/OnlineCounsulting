using OnlineConsulting.UserInterface.Features.Category;

namespace OnlineConsulting.UserInterface.Features.Home;

/// <summary>Api orchestration for the two home-page widgets that don't already have their own dedicated,
/// Api-backed service (Categories, Our Services/featured) - the rest of the home widgets
/// (Slider/WhatWeProvide/HowIGetService/Testimonials) each have their own, e.g. ISliderItemService.</summary>
public interface IHomeContentService
{
    Task<List<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<HomeFeaturedServiceViewModel>> GetFeaturedServicesAsync(CancellationToken cancellationToken = default);
}
