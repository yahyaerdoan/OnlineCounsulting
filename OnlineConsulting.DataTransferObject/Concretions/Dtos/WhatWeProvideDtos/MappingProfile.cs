using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WhatWeProvide, CreateWhatWeProvideDto>().ReverseMap();
        CreateMap<WhatWeProvide, ResultWhatWeProvideDto>().ReverseMap();
        CreateMap<WhatWeProvide, UpdateWhatWeProvideDto>().ReverseMap();
    }
}
