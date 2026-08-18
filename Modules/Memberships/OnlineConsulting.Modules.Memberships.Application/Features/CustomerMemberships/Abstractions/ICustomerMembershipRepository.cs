using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.Abstractions;

public interface ICustomerMembershipRepository : IAsyncRepository<CustomerMembership, Guid>
{
}
