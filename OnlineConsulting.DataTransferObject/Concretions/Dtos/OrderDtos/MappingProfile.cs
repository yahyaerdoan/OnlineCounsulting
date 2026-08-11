using AutoMapper;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderItemDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, CreateOrderDto>().ReverseMap();
        CreateMap<Order, ResultOrderDto>().ReverseMap();
        CreateMap<Order, UpdateOrderDto>().ReverseMap();

        // Mapping from ResultBasketItemDto to CreateOrderItemDto
        CreateMap<ResultBasketItemDto, CreateOrderItemDto>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price)).ReverseMap();
    }
}
