using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;

public interface IMembershipPlanRepository : IAsyncRepository<MembershipPlan, Guid>
{
}
