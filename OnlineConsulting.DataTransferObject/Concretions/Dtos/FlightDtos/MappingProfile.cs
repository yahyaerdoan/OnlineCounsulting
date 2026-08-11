using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.FlightDtos;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Flight, CreateFlightDto>().ReverseMap();
        CreateMap<Flight, ResultFlightDto>()
          .ForMember(dest => dest.FlightStatus,
                     opt => opt.MapFrom(src => src.FlightStatus.ToString()));
        CreateMap<Flight, UpdateFlightDto>().ReverseMap();
    }
}
