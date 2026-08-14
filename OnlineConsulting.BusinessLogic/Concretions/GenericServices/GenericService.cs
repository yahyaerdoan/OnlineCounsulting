using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.BaseEntities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
using System.Linq.Expressions;

namespace OnlineConsulting.BusinessLogic.Concretions.GenericServices;

public class GenericService<TEntity, TDto>(IMapper mapper, IGenericRepository<TEntity> repository) : IGenericService<TEntity, TDto> where TEntity : BaseEntity where TDto : class, IDto
{
    protected readonly IGenericRepository<TEntity> _repository = repository;
    protected readonly IMapper _mapper = mapper;
    protected DbSet<TEntity> Entity => _repository.Entity;
    public async Task<IOperationResult> AddAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorResult($"The {entityDisplayName} could not be created. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        await _repository.AddAsync(entity);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} has been successfully created.", ResultStatus.Created);
    }
    public async Task<IOperationResult<Guid>> CreateAndReturnIdAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorDataResult<Guid>($"The {entityDisplayName} could not be created. Please ensure the provided data is correct and try again.", ResultStatus.BadRequest);

        var returnedId = await _repository.CreateAndReturnIdAsync(entity);
        await _repository.SaveAsync();

        return new SuccessDataResult<Guid>(returnedId, $"{entityDisplayName} data created successfully.", ResultStatus.Created);
    }
    public async Task<IOperationResult> AddRangeAsync(List<TDto> datas)
    {
        var entityName = _repository.Entity?.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (datas is null || datas.Count == 0)
            return new ErrorResult($"No valid {entityName} data was provided for saving. Please ensure the data is correct and try again.", ResultStatus.BadRequest);

        var entities = _mapper.Map<List<TEntity>>(datas);

        var entityDisplayName = entities.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (entities is null || datas.Count == 0)
            return new ErrorResult($"An error occurred while processing the {entityDisplayName} data. Please verify the input and try again.", ResultStatus.BadRequest);

        await _repository.AddRangeAsync(entities);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} data has been successfully saved.", ResultStatus.Created);
    }
    public async Task<IOperationResult<IQueryable<TResult>>> GetAllAsync<TResult>(bool tracking = true, bool? status = true)
    {
        var entities = _repository.GetAll(tracking, status).OrderByDescending(x => x.CreatedDate);

        var entityDisplayName = (await entities.FirstOrDefaultAsync())?.EntityName ?? typeof(TEntity).Name;

        if (!await entities.AnyAsync())
            return new ErrorDataResult<IQueryable<TResult>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TResult>(entities);

        return new SuccessDataResult<IQueryable<TResult>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<TResult>> GetByIdAsync<TResult>(string id, bool traking = true, bool? status = true)
    {
        var entity = await _repository.GetByIdAsync(id: id, traking: traking, status: status);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorDataResult<TResult>($"The {entityDisplayName} with ID '{id}' could not be found. Please verify the ID and try again.", ResultStatus.NotFound);

        var result = _mapper.Map<TResult>(entity);

        return new SuccessDataResult<TResult>(result, $"{entityDisplayName} retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<TResult>> GetSingleAsync<TResult>(Expression<Func<TResult, bool>> expression, bool traking = true, bool? status = true)
    {
        var entityExpression = _mapper.MapExpression<Expression<Func<TEntity, bool>>>(expression);

        var entity = await _repository.GetSingleAsync(expression: entityExpression, traking: traking, status: status);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorDataResult<TResult>($"No {entityDisplayName} matching the criteria could be found.", ResultStatus.NotFound);

        var result = _mapper.Map<TResult>(entity);
        return new SuccessDataResult<TResult>(result, $"{entityDisplayName} retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<IQueryable<TResult>>> GetWhereAsync<TResult>(Expression<Func<TResult, bool>> expression, bool traking = true, bool? status = true)
    {
        var entityExpression = _mapper.MapExpression<Expression<Func<TEntity, bool>>>(expression);

        var entities = _repository.GetWhere(expression: entityExpression, traking: traking, status: status);

        var entityDisplayName = (await entities.FirstOrDefaultAsync())?.EntityName ?? typeof(TEntity).Name;

        if (!await entities.AnyAsync())
            return new ErrorDataResult<IQueryable<TResult>>($"No {entityDisplayName} matching the criteria could be found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TResult>(entities);

        return new SuccessDataResult<IQueryable<TResult>>(result, $"{entityDisplayName} entities retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> RemoveAsync(TDto dto, bool isSoftDelete = true)
    {
        var entity = _mapper.Map<TEntity>(dto);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorResult($"The {entityDisplayName} could not be identified for deletion. Please verify the provided details and try again.", ResultStatus.BadRequest);

        await _repository.RemoveAsync(entity, isSoftDelete);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} has been successfully deleted.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> RemoveByIdAsync(string id, bool isSoftDelete = true)
    {
        var entity = _repository.Entity.Find(Guid.Parse(id));

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorResult($"The {entityDisplayName} with the specified ID could not be found. Please check the ID and try again.", ResultStatus.NotFound);

        await _repository.RemoveAsync(entity, isSoftDelete);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} was successfully deleted.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> RemoveRange(List<TDto> datas, bool isSoftDelete = true)
    {
        var entityName = _repository.Entity?.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (datas is null || datas.Count == 0)
            return new ErrorResult($"No {entityName} data was provided for removal. Please ensure the data is correct and try again.", ResultStatus.BadRequest);

        var entities = _mapper.Map<List<TEntity>>(datas);

        var entityDisplayName = entities.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (entities is null || entities.Count == 0)
            return new ErrorResult($"An error occurred while processing the {entityDisplayName} data for removal. Please verify the input and try again.", ResultStatus.BadRequest);

        _repository.RemoveRange(entities, isSoftDelete);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} data has been successfully removed.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<int>> SaveAsync()
    {
        var affected = await _repository.SaveAsync();

        return new SuccessDataResult<int>(affected, "Changes have been saved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);

        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorResult($"Failed to map the provided {entityDisplayName} data. Please ensure the input is valid and try again.", ResultStatus.BadRequest);

        entity.Status = true;

        await _repository.UpdateAsync(entity);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} has been successfully updated.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> UpdateRange(List<TDto> datas)
    {
        var entityName = _repository.Entity?.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (datas is null || datas.Count == 0)
            return new ErrorResult($"No {entityName} data was provided for update. Please ensure the input is valid and try again.", ResultStatus.BadRequest);

        var entities = _mapper.Map<List<TEntity>>(datas);

        var entityDisplayName = entities.FirstOrDefault()?.EntityName ?? typeof(TEntity).Name;

        if (entities is null || entities.Count == 0)
            return new ErrorResult($"An error occurred while processing the {entityDisplayName} data for update. Please verify the input and try again.", ResultStatus.BadRequest);

        _repository.UpdateRange(entities);
        await _repository.SaveAsync();

        return new SuccessResult($"The {entityDisplayName} data has been successfully updated.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<TResult>> GetFirstOrDefaultAsync<TResult>(Expression<Func<TResult, bool>> expression, bool tracking = true, bool? status = true)
    {
        var entityExpression = _mapper.MapExpression<Expression<Func<TEntity, bool>>>(expression);

        var entity = await _repository.GetFirstOrDefaultAsync(entityExpression, tracking, status);
        var entityDisplayName = entity?.EntityName ?? typeof(TEntity).Name;

        if (entity is null)
            return new ErrorDataResult<TResult>($"No {entityDisplayName} matching the criteria could be found.", ResultStatus.NotFound);

        var result = _mapper.Map<TResult>(entity);
        return new SuccessDataResult<TResult>(result, $"{entityDisplayName} retrieved successfully.", ResultStatus.Ok);
    }
}
