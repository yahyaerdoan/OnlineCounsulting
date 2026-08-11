using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.Linq.Expressions;

namespace OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<bool> AddAsync(T entity);
    Task<Guid> CreateAndReturnIdAsync(T entity);
    Task<bool> AddRangeAsync(List<T> datas);
    Task<bool> RemoveAsync(T entity, bool isSoftDelete = true);
    Task<bool> RemoveByIdAsync(string id, bool isSoftDelete = true);
    Task<bool> RemoveByIdAsync(Guid id, bool isSoftDelete = true);
    bool RemoveRange(List<T> datas, bool isSoftDelete = true);
    Task<bool> UpdateAsync(T entity);
    bool UpdateRange(List<T> datas);
    IQueryable<T> GetAll(bool traking = true, bool? status = true);
    IQueryable<T> GetWhere(Expression<Func<T, bool>> expression, bool traking = true, bool? status = true);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> expression, bool traking = true, bool? status = true);
    Task<T?> GetByIdAsync(string id, bool traking = true, bool? status = true);
    Task<int> SaveAsync();
    DbSet<T> Entity { get; }
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = true, bool? status = true);
}
