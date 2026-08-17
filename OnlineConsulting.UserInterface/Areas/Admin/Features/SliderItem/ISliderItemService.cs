using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

/// <summary>All Api orchestration for the SliderItem admin screens (Api concept is called HeroSlide - the
/// C# type/route name stays SliderItem for continuity with the existing view folder).</summary>
public interface ISliderItemService
{
    Task<List<SliderItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateSliderItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateSliderItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateSliderItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
