using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.NewsletterDtos;

public class CreateNewsletterDto : IDto
{
    public string Email { get; set; } = string.Empty;
}
