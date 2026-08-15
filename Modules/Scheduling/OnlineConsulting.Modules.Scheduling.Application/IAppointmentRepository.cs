using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application;

public interface IAppointmentRepository : IAsyncRepository<Appointment, Guid>
{
}
