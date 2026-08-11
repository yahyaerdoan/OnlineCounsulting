using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FooterAbout, CreateFooterAboutDto>().ReverseMap();
        CreateMap<FooterAbout, ResultFooterAboutDto>().ReverseMap();
        CreateMap<FooterAbout, UpdateFooterAboutDto>().ReverseMap();
    }
}
