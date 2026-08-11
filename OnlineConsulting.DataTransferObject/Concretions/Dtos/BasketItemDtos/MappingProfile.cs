using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BasketItem, CreateBasketItemDto>().ReverseMap();
        CreateMap<BasketItem, ResultBasketItemDto>().ReverseMap();
        CreateMap<BasketItem, UpdateBasketItemDto>().ReverseMap();
    }
}
