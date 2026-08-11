using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BookDtos;

public class ResultBookDto : IDto
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
    public bool Status { get; set; }
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
