using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Service, CreateServiceDto>().ReverseMap();
        CreateMap<Service, ResultServiceDto>().ReverseMap();
        CreateMap<Service, UpdateServiceDto>().ReverseMap();
        CreateMap<Service, ResultServiceWithImageDto>().ReverseMap();
    }
}
