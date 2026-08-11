using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GalleryItem, CreateGalleryItemDto>().ReverseMap();
        CreateMap<GalleryItem, ResultGalleryItemDto>().ReverseMap();
        CreateMap<GalleryItem, UpdateGalleryItemDto>().ReverseMap();

        CreateMap<GalleryItem, UpdateGalleryItemWithCategoryDto>().ForMember(dest => dest.GalleryCategories, opt => opt.MapFrom(src => src.GalleryCategories.Select(gc => gc.GalleryCategory)));

        CreateMap<GalleryItem, ResultGalleryItemWithCategoriesDto>()
     .ForMember(dest => dest.GalleryCategories, opt => opt.MapFrom(src => src.GalleryCategories.Select(gc => gc.GalleryCategory)));
    }
}
