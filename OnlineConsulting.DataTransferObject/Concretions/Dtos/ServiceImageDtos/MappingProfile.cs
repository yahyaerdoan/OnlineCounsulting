using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ServiceImage, CreateServiceImageDto>().ReverseMap();
        CreateMap<ServiceImage, ResultServiceImageDto>().ReverseMap();
        CreateMap<ServiceImage, UpdateServiceImageDto>().ReverseMap();
    }
}
