using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IPartnershipSocialMediaService : IGenericService<PartnershipSocialMedia, IDto>
{
    Task<IOperationResult<IQueryable<TDto>>> GetAllSocialMediasByParnershipIdAsync<TDto>(string id, bool tracking = true, bool? status = true);
}
