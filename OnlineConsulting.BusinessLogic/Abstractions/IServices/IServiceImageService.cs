using Microsoft.AspNetCore.Http;
using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IServiceImageService : IGenericService<ServiceImage, IDto>
{
    Task<IOperationResult> AddServiceImageAsync(CreateServiceImageDto dto);
    Task<IOperationResult> UpdateServiceImageAsync(string id, IFormFile image);
    Task<IOperationResult> RemoveServiceImageByIdAsync(string id, bool isSoftDelete = true);
}
