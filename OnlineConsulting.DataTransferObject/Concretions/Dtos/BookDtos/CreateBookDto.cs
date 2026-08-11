using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

public class CreateBookDto : IDto
{
    public Guid UserId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingStatus { get; set; } = "Pending"; //TODO : Booking status base entityden gelen statusle çakışıyor. Entity de düzenleme yapılacak
    public string PaymentStatus { get; set; } = "Unpaid";
    public string Notes { get; set; } = string.Empty;
}
