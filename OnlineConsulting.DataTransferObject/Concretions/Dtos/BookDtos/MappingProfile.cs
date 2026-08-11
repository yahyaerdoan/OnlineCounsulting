using AutoMapper;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, CreateBookDto>().ReverseMap();
        CreateMap<Book, ResultBookDto>().ReverseMap();
        CreateMap<Book, UpdateBookDto>().ReverseMap();
    }
}
