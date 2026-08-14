using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;

public class ResultBasketDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public ICollection<BasketItem> BasketItems { get; set; } = [];

    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
}
