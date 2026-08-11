using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, ResultUserDto>().ReverseMap();
        CreateMap<User, UpdateUserDto>().ReverseMap();
    }
}
