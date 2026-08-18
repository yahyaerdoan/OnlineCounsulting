using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Abstractions;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Repositories;

public class AppointmentMediaItemRepository(SchedulingDbContext context) : EfRepositoryBase<AppointmentMediaItem, Guid, SchedulingDbContext>(context), IAppointmentMediaItemRepository
{
}
