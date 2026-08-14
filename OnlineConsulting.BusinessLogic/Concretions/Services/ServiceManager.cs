using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IStorages;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataAccess.Abstractions.IRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
using ResultHandler.Core.Enums;
using ResultHandler.Facade;
using ResultHandler.Implementations.Error;
using ResultHandler.Implementations.Success;
namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class ServiceManager(IMapper mapper, IGenericRepository<Service> repository, IStorageService _storageService, IServiceImageService serviceImageService, IServiceRepository serviceRepository) : GenericService<Service, IDto>(mapper, repository), IServiceService
{
    public async Task<IOperationResult> AddServiceWithImagesAsync(CreateServiceDto dto)
    {
        if (dto.Images is null || !dto.Images.Any())
            return new ErrorResult("At least one image must be provided.", ResultStatus.BadRequest);
        else
        {
            foreach (var image in dto.Images)
            {
                var uploadedImageRoute = await _storageService.UploadAsync("Resource/LocalStorage/Service-Images", image);
                if (!string.IsNullOrEmpty(uploadedImageRoute.Data.FullPath))
                {
                    dto.ServiceImagesUrlList.Add($"/{uploadedImageRoute.Data.TargetFolderPathOrContainerName}/{uploadedImageRoute.Data.FileName}");
                }
                else
                    return new ErrorResult($"An error occurred while creating the service image", ResultStatus.BadRequest);
            }
        }
        #region OpenAi Content create imp
        //var result = await aiContentService.GenerateServiceContentAsync(dto.Title);
        //dto.Title = result.slug;
        //dto.Description = result.description;
        #endregion

        var slugPoint = SlugHelper.GenerateSlug(dto.Title);
        var slug = slugPoint;
        var i = 1;

        while (await _repository.GetWhere(x => x.Slug == slug).AnyAsync())
        {
            slug = $"{slugPoint}-{i}";
            i++;
        }

        dto.Slug = slug;

        var serviceDataResult = await CreateAndReturnIdAsync(dto);
        List<IDto> serviceImagesDtoList = [];
        if (serviceDataResult.IsSuccessful)
        {
            foreach (var imageUrl in dto.ServiceImagesUrlList)
            {
                var imageDto = new CreateServiceImageDto()
                {
                    ImageUrl = imageUrl,
                    ServiceId = serviceDataResult.Data,
                };
                serviceImagesDtoList.Add(imageDto);
            }
            await serviceImageService.AddRangeAsync(serviceImagesDtoList);
        }
        return serviceDataResult;
    }
    public async Task<IOperationResult<IQueryable<TDto>>> GetAllServicesWithImagesAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var entities = serviceRepository.GetAllServicesWithImages(tracking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(Service).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<IQueryable<TDto>>> GetAllServicesByFeaturedAreaTrueAsync<TDto>(bool tracking = true, bool? status = true)
    {
        var entities = serviceRepository.GetAllServicesByFeaturedAreaTrue(tracking, status);

        var firstEntity = await entities.FirstOrDefaultAsync();
        var entityDisplayName = firstEntity?.EntityName ?? typeof(Service).Name;

        if (firstEntity is null)
            return new ErrorDataResult<IQueryable<TDto>>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<TDto>(entities);

        return new SuccessDataResult<IQueryable<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<ResultServiceWithPagedDto>> GetAllServicesWithPagedAsync(int size, int page, bool tracking = true, bool? status = true)
    {
        var entities = serviceRepository.GetAllServicesWithPaged(size, page, tracking, status);

        var count = await Entity.CountAsync();
        var firstEntity = await entities.FirstOrDefaultAsync();

        var entityDisplayName = firstEntity?.EntityName ?? typeof(Service).Name;

        if (firstEntity is null)
            return new ErrorDataResult<ResultServiceWithPagedDto>($"No {entityDisplayName} data found.", ResultStatus.NotFound);

        var result = _mapper.ProjectTo<ResultServiceWithImageDto>(entities);

        var pagedAndCountedServices = new ResultServiceWithPagedDto()
        {
            Page = page,
            Size = size,
            TotalCount = count,
            Services = result
        };
        return new SuccessDataResult<ResultServiceWithPagedDto>(pagedAndCountedServices, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<List<TDto>>> GetServicesWithImagesByCategoryIdAsync<TDto>(string id, bool traking = true, bool? status = true)
    {
        var services = serviceRepository.GetServicesWithImagesByCategoryIdAsync(id, traking, status);

        var entityDisplayName = (await services.FirstOrDefaultAsync())?.EntityName ?? typeof(Service).Name;

        if (services is null)
            return new ErrorDataResult<List<TDto>>("No services found for the given category.", ResultStatus.NotFound);

        var serviceList = await services.ToListAsync();
        var result = _mapper.Map<List<TDto>>(serviceList);

        return new SuccessDataResult<List<TDto>>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<TDto>> GetServiceWithImagesByIdAsync<TDto>(string id, bool traking = true, bool? status = true)
    {
        var entity = await serviceRepository.GetServiceWithImagesByIdAsync(id, traking, status);

        if (entity is null)
            return new ErrorDataResult<TDto>($"No service data  found.", ResultStatus.NotFound);

        var entityDisplayName = entity.EntityName ?? typeof(Service).Name;

        var result = _mapper.Map<TDto>(entity);

        return new SuccessDataResult<TDto>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult> RemoveServiceByIdAsync(string id, bool isSoftDelete = true)
    {
        if (isSoftDelete)
        {
            var result = await RemoveByIdAsync(id, true);
            return result;
        }
        else
        {
            var serviceImageListResult = await serviceImageService.GetWhereAsync<ResultServiceImageDto>(x => x.ServiceId == Guid.Parse(id));

            var result = await RemoveByIdAsync(id, false);
            if (result.IsSuccessful && serviceImageListResult.IsSuccessful)
            {
                foreach (var serviceImage in serviceImageListResult.Data)
                {
                    await _storageService.DeleteAsync(serviceImage.ImageUrl);
                }
            }
            return result;
        }
    }
    public async Task<IOperationResult<TDto>> GetServiceBySlugAsync<TDto>(string slug, bool traking = true, bool? status = true)
    {
        var entity = await serviceRepository.GetServiceBySlugWithImagesAndCategorydAsync(slug, traking, status);

        if (entity is null)
            return new ErrorDataResult<TDto>($"No service data  found.", ResultStatus.NotFound);

        var entityDisplayName = entity.EntityName ?? typeof(Service).Name;

        var result = _mapper.Map<TDto>(entity);

        return new SuccessDataResult<TDto>(result, $"{entityDisplayName} data retrieved successfully.", ResultStatus.Ok);
    }
    public async Task<IOperationResult<List<TDto>>> SearchServicesAsync<TDto>(string query, bool traking = true, bool? status = true)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Result.Success<List<TDto>>([], "No search query provided.");

        var entities = serviceRepository.SearchServicesWithImages(query, traking, status);

        var serviceList = await entities.ToListAsync();
        var result = _mapper.Map<List<TDto>>(serviceList);

        return Result.Success(result, serviceList.Count == 0
            ? "No services matched the search query."
            : "Services retrieved successfully.");
    }
}
