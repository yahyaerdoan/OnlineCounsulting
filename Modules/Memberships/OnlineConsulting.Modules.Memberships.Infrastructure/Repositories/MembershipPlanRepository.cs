using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Repositories;

public class MembershipPlanRepository(MembershipsDbContext context) : EfRepositoryBase<MembershipPlan, Guid, MembershipsDbContext>(context), IMembershipPlanRepository
{
}
