using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SliderItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;
using ResultHandler.Core.Abstractions;
namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface ISliderItemService : IGenericService<SliderItem, IDto>
{
    Task<IOperationResult> AddSliderItemAsync(CreateSliderItemDto dto);
    Task<IOperationResult> UpdateSliderItemImageAsync(string id, Microsoft.AspNetCore.Http.IFormFile image);
    Task<IOperationResult> RemoveSliderItemByIdAsync(string id, bool isSoftDelete = true);
}
