using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Message, CreateMessageDto>().ReverseMap();
        CreateMap<Message, ResultMessageDto>().ReverseMap();
        CreateMap<Message, UpdateMessageDto>().ReverseMap();
    }
}
