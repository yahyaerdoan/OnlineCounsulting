using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserAddress, CreateUserAddressDto>().ReverseMap();
        CreateMap<UserAddress, ResultUserAddressDto>().ReverseMap();
        CreateMap<UserAddress, UpdateUserAddressDto>().ReverseMap();
    }
}
