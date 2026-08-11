using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.NewsletterDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Newsletter, CreateNewsletterDto>().ReverseMap();
        CreateMap<Newsletter, ResultNewsletterDto>().ReverseMap();
        CreateMap<Newsletter, UpdateNewsletterDto>().ReverseMap();
    }
}
