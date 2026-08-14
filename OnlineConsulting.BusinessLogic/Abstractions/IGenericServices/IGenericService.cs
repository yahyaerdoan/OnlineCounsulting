using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.BaseEntities;
using ResultHandler.Core.Abstractions;
using System.Linq.Expressions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;

public interface IGenericService<Tentity, TDto> where Tentity : BaseEntity where TDto : class, IDto
{
    Task<IOperationResult> AddAsync(TDto dto);
    Task<IOperationResult<Guid>> CreateAndReturnIdAsync(TDto dto);
    Task<IOperationResult> AddRangeAsync(List<TDto> datas);
    Task<IOperationResult> RemoveAsync(TDto dto, bool isSoftDelete = true);
    Task<IOperationResult> RemoveByIdAsync(string id, bool isSoftDelete = true);
    Task<IOperationResult> RemoveRange(List<TDto> datas, bool isSoftDelete = true);
    Task<IOperationResult> UpdateAsync(TDto dto);
    Task<IOperationResult> UpdateRange(List<TDto> datas);
    Task<IOperationResult<IQueryable<TResult>>> GetAllAsync<TResult>(bool tracking = true, bool? status = true);
    Task<IOperationResult<IQueryable<TResult>>> GetWhereAsync<TResult>(Expression<Func<TResult, bool>> expression, bool tracking = true, bool? status = true);
    Task<IOperationResult<TResult>> GetSingleAsync<TResult>(Expression<Func<TResult, bool>> expression, bool tracking = true, bool? status = true);
    Task<IOperationResult<TResult>> GetByIdAsync<TResult>(string id, bool traking = true, bool? status = true);
    Task<IOperationResult<int>> SaveAsync();
    Task<IOperationResult<TResult>> GetFirstOrDefaultAsync<TResult>(Expression<Func<TResult, bool>> expression, bool tracking = true, bool? status = true);
}
