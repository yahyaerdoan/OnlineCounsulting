using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Role, CreateSystemRoleDto>().ReverseMap();
        CreateMap<Role, ResultSystemRoleDto>().ReverseMap();
        CreateMap<Role, UpdateSystemRoleDto>().ReverseMap();
    }
}
