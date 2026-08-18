using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;

public interface IWorkOrderMediaItemRepository : IAsyncRepository<WorkOrderMediaItem, Guid>
{
}
