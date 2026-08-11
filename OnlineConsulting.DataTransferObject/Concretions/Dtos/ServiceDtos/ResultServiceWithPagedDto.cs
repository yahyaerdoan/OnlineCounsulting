using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

public class ResultServiceWithPagedDto : IDto
{
    public int TotalCount { get; set; }
    public int Size { get; set; }
    public int Page { get; set; }
    public IQueryable<ResultServiceWithImageDto> Services { get; set; } = Enumerable.Empty<ResultServiceWithImageDto>().AsQueryable();
}
