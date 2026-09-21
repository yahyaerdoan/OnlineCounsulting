using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

/// <summary>All Api orchestration for the ProvidedItem admin screens (Api concept is called
/// ServiceOffering - the C# type/route name stays ProvidedItem for continuity with the existing view folder).</summary>
public interface IProvidedItemService
{
    /// <summary>Fetches all provided items.</summary>
    Task<List<ProvidedItemListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Fetches a single provided item for editing, or null if not found.</summary>
    Task<UpdateProvidedItemViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new provided item.</summary>
    Task<ApiEnvelope> CreateAsync(CreateProvidedItemViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing provided item.</summary>
    Task<ApiEnvelope> UpdateAsync(UpdateProvidedItemViewModel model, CancellationToken cancellationToken = default);

    /// <summary>Deletes a provided item by id.</summary>
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
