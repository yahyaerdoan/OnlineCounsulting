using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ImgIconDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IImgIconService : IGenericService<ImgIcon, IDto>
{
    Task<IOperationResult> AddImgIconAsync(CreateImgIconDto dto);
    Task<IOperationResult> UpdateImgIconImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemoveImgIconByIdAsync(string id, bool isSoftDelete = true);
}
