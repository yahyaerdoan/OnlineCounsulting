using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IHowIGetServiceService : IGenericService<HowIGetService, IDto>
{
    Task<IOperationResult<IQueryable<TDto>>> GetAllHowIGetServicesWithImgIconsAsync<TDto>(bool tracking = true, bool? status = true);
}
