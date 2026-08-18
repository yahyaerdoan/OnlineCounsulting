using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Abstractions;

public interface IAppointmentMediaItemRepository : IAsyncRepository<AppointmentMediaItem, Guid>
{
}
