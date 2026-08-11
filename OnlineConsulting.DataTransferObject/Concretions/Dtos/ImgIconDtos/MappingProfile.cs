using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ImgIconDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ImgIcon, CreateImgIconDto>().ReverseMap();
        CreateMap<ImgIcon, ResultImgIconDto>().ReverseMap();
        CreateMap<ImgIcon, UpdateImgIconDto>().ReverseMap();
    }
}
