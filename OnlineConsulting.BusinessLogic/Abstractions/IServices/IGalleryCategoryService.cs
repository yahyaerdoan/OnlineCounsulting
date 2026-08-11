using OnlineConsulting.BusinessLogic.Abstractions.IGenericServices;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IGalleryCategoryService : IGenericService<GalleryCategory, IDto>
{
}
