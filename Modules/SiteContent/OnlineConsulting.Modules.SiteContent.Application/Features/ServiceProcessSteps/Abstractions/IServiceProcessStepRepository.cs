using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.SiteContent.Domain.Service;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Abstractions;

public interface IServiceProcessStepRepository : IAsyncRepository<ServiceProcessStep, Guid>
{
}
