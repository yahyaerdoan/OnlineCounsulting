using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;
using OnlineConsulting.Modules.Memberships.Domain;
using OnlineConsulting.Modules.Memberships.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Memberships.Infrastructure.Repositories;

public class CustomerMembershipRepository(MembershipsDbContext context) : EfRepositoryBase<CustomerMembership, Guid, MembershipsDbContext>(context), ICustomerMembershipRepository
{
}
