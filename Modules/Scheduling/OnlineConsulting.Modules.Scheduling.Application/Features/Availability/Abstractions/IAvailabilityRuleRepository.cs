using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;

public interface IAvailabilityRuleRepository : IAsyncRepository<AvailabilityRule, Guid>
{
}
