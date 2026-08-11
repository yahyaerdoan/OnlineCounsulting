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

public class CategoryManager(IMapper mapper, IGenericRepository<Category> repository, ICategoryRepository categoryRepository) : GenericService<Category, IDto>(mapper, repository), ICategoryService
{
    public async Task<IOperationResult<IQueryable<TDto>>> GetAllCategoriesWithImgIconsAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var entities = categoryRepository.GetAllCategoriesWithImgIcons(tracking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(ProvidedItem).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
}
