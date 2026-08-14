using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class ProvidedItemManager(IMapper mapper, IGenericRepository<ProvidedItem> repository, IProvidedItemRepository providedItemRepository) : GenericService<ProvidedItem, IDto>(mapper, repository), IProvidedItemService
{
    public async Task<IOperationResult<IQueryable<TDto>>> GetAllProvidedItemsWithImgIconsAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var entities = providedItemRepository.GetAllProvidedItemsWithImgIcons(tracking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(ProvidedItem).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
}
