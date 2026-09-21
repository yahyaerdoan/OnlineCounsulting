using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SliderItem;

/// <summary>All Api orchestration for the SliderItem admin screens (Api concept is called HeroSlide - the
/// C# type/route name stays SliderItem for continuity with the existing view folder).</summary>
public interface ISliderItemService
{
    /// <summary>Fetches all slider items.</summary>
    Task<List<SliderItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single slider item for editing, or null if not found.</summary>
    Task<UpdateSliderItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new slider item.</summary>
    Task<ApiEnvelope> CreateAsync(CreateSliderItemViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates a slider item, keeping the existing image when none is uploaded.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateSliderItemViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a slider item by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
