using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ClassIcon, CreateClassIconDto>().ReverseMap();
        CreateMap<ClassIcon, ResultClassIconDto>().ReverseMap();
        CreateMap<ClassIcon, UpdateClassIconDto>().ReverseMap();
    }
}
