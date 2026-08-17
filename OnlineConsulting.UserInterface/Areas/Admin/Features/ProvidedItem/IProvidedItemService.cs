using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

/// <summary>All Api orchestration for the ProvidedItem admin screens (Api concept is called
/// ServiceOffering - the C# type/route name stays ProvidedItem for continuity with the existing view folder).</summary>
public interface IProvidedItemService
{
    Task<List<ProvidedItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateProvidedItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateProvidedItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateProvidedItemViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
