using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ProvidedItemDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProvidedItem, CreateProvidedItemDto>().ReverseMap();
        CreateMap<ProvidedItem, ResultProvidedItemDto>().ReverseMap();
        CreateMap<ProvidedItem, UpdateProvidedItemDto>().ReverseMap();
    }
}
