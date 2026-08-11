using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.DataAccess.Abstractions.IUnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}
