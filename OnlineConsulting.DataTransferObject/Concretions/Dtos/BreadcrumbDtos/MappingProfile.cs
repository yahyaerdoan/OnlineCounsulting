using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BreadcrumbDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Breadcrumb, CreateBreadcrumbDto>().ReverseMap();
        CreateMap<Breadcrumb, ResultBreadcrumbDto>().ReverseMap();
        CreateMap<Breadcrumb, UpdateBreadcrumbDto>().ReverseMap();
    }
}
