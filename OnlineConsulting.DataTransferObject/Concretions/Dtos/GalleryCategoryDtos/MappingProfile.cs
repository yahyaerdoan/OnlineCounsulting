using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GalleryCategory, CreateGalleryCategoryDto>().ReverseMap();
        CreateMap<GalleryCategory, ResultGalleryCategoryDto>().ReverseMap();
        CreateMap<GalleryCategory, UpdateGalleryCategoryDto>().ReverseMap();
    }
}
