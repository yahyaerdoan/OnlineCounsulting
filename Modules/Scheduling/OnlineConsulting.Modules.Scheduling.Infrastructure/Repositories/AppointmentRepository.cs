using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Scheduling.Application;
using OnlineConsulting.Modules.Scheduling.Domain;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure.Repositories;

public class AppointmentRepository(SchedulingDbContext context) : EfRepositoryBase<Appointment, Guid, SchedulingDbContext>(context), IAppointmentRepository
{
}
