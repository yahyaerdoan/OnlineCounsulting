using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IServiceService : IGenericService<Service, IDto>
{
    Task<IOperationResult> AddServiceWithImagesAsync(CreateServiceDto dto);
    Task<IOperationResult> RemoveServiceByIdAsync(string id, bool isSoftDelete = true);
    Task<IOperationResult<IQueryable<TDto>>> GetAllServicesWithImagesAsync<TDto>(bool tracking = true, bool? status = true);
    Task<IOperationResult<IQueryable<TDto>>> GetAllServicesByFeaturedAreaTrueAsync<TDto>(bool tracking = true, bool? status = true);
    Task<IOperationResult<ResultServiceWithPagedDto>> GetAllServicesWithPagedAsync(int size, int page, bool tracking = true, bool? status = true);
    Task<IOperationResult<TDto>> GetServiceWithImagesByIdAsync<TDto>(string id, bool traking = true, bool? status = true);
    Task<IOperationResult<List<TDto>>> GetServicesWithImagesByCategoryIdAsync<TDto>(string id, bool traking = true, bool? status = true);
    Task<IOperationResult<TDto>> GetServiceBySlugAsync<TDto>(string slug, bool traking = true, bool? status = true);
    Task<IOperationResult<List<TDto>>> SearchServicesAsync<TDto>(string query, bool traking = true, bool? status = true);
}
