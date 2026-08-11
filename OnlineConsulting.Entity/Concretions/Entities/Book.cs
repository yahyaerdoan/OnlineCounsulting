using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Book : BaseEntity
{
    //TODO:  Yeniden gozden gecirilecek
    public Guid ServiceId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string BookingStatus { get; set; } = "Pending";
    public string PaymentStatus { get; set; } = "Unpaid";
    public string Notes { get; set; } = string.Empty;
    public Service Service { get; set; } = default!;
    [NotMapped]
    public override string EntityName => "Reservation Process";
}

