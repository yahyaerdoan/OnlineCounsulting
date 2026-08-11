using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IFooterAboutService : IGenericService<FooterAbout, IDto>
{
    Task<IOperationResult> AddFooterAboutAsync(CreateFooterAboutDto dto);
    Task<IOperationResult> UpdateFooterAboutImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemoveFooterAboutByIdAsync(string id, bool isSoftDelete = true);
}
