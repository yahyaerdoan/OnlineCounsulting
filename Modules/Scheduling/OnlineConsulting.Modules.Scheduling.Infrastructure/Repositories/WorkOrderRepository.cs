using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Repositories;

public class WorkOrderRepository(SchedulingDbContext context) : EfRepositoryBase<WorkOrder, Guid, SchedulingDbContext>(context), IWorkOrderRepository
{
}
