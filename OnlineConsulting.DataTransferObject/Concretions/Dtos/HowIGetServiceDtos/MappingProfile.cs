using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.HowIGetServiceDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<HowIGetService, CreateHowIGetServiceDto>().ReverseMap();
        CreateMap<HowIGetService, ResultHowIGetServiceDto>().ReverseMap();
        CreateMap<HowIGetService, UpdateHowIGetServiceDto>().ReverseMap();
    }
}
