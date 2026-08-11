using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.SliderItemDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SliderItem, CreateSliderItemDto>().ReverseMap();
        CreateMap<SliderItem, ResultSliderItemDto>().ReverseMap();
        CreateMap<SliderItem, UpdateSliderItemDto>().ReverseMap();
    }
}
