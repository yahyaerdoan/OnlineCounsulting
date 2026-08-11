using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

public class UpdateBookDto : IDto
{
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public Guid UserId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
