using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PartnershipSocialMedia, CreatePartnershipSocialMediaDto>().ReverseMap();
        CreateMap<PartnershipSocialMedia, ResultPartnershipSocialMediaDto>().ReverseMap();
        CreateMap<PartnershipSocialMedia, UpdatePartnershipSocialMediaDto>().ReverseMap();
    }
}
