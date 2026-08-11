using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BreadcrumbDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IBreadcrumbService : IGenericService<Breadcrumb, IDto>
{
    Task<IOperationResult> AddBreadcrumbAsync(CreateBreadcrumbDto dto);
    Task<IOperationResult> UpdateBreadcrumbImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemoveBreadcrumbByIdAsync(string id, bool isSoftDelete = true);
}
