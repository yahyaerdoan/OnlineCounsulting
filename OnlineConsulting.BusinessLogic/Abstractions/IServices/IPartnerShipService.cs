using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IPartnershipService : IGenericService<Partnership, IDto>
{
    Task<IOperationResult> AddPartnershipAsync(CreatePartnershipDto dto);
    Task<IOperationResult> UpdatePartnershipImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemovePartnershipByIdAsync(string id, bool isSoftDelete = true);
    Task<IOperationResult<IQueryable<TDto>>> GetAllPartnershipsWithSocialMediasAsync<TDto>(bool tracking = true, bool? status = true);
}
