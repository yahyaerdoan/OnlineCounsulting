using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Basket, CreateBasketDto>().ReverseMap();
        CreateMap<Basket, ResultBasketDto>().ReverseMap();
        CreateMap<Basket, UpdateBasketDto>().ReverseMap();
    }
}
