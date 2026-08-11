using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.NewsletterDtos;

public class UpdateNewsletterDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; } = string.Empty;
}
