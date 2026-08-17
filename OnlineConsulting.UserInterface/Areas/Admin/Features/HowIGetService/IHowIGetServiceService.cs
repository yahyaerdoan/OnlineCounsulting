using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.HowIGetService;

/// <summary>All Api orchestration for the HowIGetService admin screens (Api concept is called
/// ServiceProcessStep - the C# type/route name stays HowIGetService for continuity with the existing view folder).</summary>
public interface IHowIGetServiceService
{
    Task<List<HowIGetServiceListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateHowIGetServiceViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateHowIGetServiceViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateHowIGetServiceViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
