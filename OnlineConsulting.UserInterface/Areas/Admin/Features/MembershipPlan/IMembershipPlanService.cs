using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.MembershipPlan;

/// <summary>All Api orchestration for the MembershipPlan admin screens. The Api has no Delete endpoint for
/// this resource, and Update cannot change BillingCycle/Price - only Create/Read/Update(partial) exist.</summary>
public interface IMembershipPlanService
{
    Task<List<MembershipPlanListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UpdateMembershipPlanViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> CreateAsync(CreateMembershipPlanViewModel model, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> UpdateAsync(UpdateMembershipPlanViewModel model, CancellationToken cancellationToken = default);
    Task<List<MembershipSubscriberListItemViewModel>> GetSubscribersAsync(CancellationToken cancellationToken = default);
}
