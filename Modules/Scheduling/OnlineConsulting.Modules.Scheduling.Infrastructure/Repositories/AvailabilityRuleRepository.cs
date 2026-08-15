using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Scheduling.Application;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Repositories;

public class AvailabilityRuleRepository(SchedulingDbContext context) : EfRepositoryBase<AvailabilityRule, Guid, SchedulingDbContext>(context), IAvailabilityRuleRepository
{
}
