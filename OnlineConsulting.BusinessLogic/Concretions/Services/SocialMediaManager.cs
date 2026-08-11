using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class SocialMediaManager(IMapper mapper, IGenericRepository<SocialMedia> repository, ISocialMediaRepository socialMediaRepository) : GenericService<SocialMedia, IDto>(mapper, repository), ISocialMediaService
{
    public async Task<IOperationResult<IQueryable<TDto>>> GetAllSocialMediaAccontsWithIconAsync<TDto>(bool traking = true, bool? status = true)
    {
        var entities = socialMediaRepository.GetAllSocialMediaAccontsWithIcon(traking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(SocialMedia).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
}
