using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AboutUs, CreateAboutUsDto>().ReverseMap();
        CreateMap<AboutUs, ResultAboutUsDto>().ReverseMap();
        CreateMap<AboutUs, UpdateAboutUsDto>().ReverseMap();
    }
}
