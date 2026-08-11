using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface ISocialMediaService : IGenericService<SocialMedia, IDto>
{
    Task<IOperationResult<IQueryable<TDto>>> GetAllSocialMediaAccontsWithIconAsync<TDto>(bool traking = true, bool? status = true);
}
