using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Categories.Domain;

namespace OnlineConsulting.Modules.Categories.Application.Abstractions;

public interface ICategoryRepository : IAsyncRepository<Category, Guid>
{
}
