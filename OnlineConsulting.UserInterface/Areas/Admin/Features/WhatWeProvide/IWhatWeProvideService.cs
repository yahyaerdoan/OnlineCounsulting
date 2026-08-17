using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.WhatWeProvide;

/// <summary>All Api orchestration for the WhatWeProvide admin screens (Api concept is called FeatureHighlight -
/// the C# type/route name stays WhatWeProvide for continuity with the existing view folder).</summary>
public interface IWhatWeProvideService
{
    Task<List<WhatWeProvideListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateWhatWeProvideViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateWhatWeProvideViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateWhatWeProvideViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
