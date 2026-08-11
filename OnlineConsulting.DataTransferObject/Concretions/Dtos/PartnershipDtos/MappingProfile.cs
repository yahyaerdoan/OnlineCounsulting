using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Partnership, CreatePartnershipDto>().ReverseMap();
        CreateMap<Partnership, ResultPartnershipDto>().ReverseMap();
        CreateMap<Partnership, UpdatePartnershipDto>().ReverseMap();
    }
}
