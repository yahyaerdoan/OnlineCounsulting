using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;

public class ResultBasketItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public Guid BasketId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }


    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }
    public bool Status { get; set; }
    public Service Service { get; set; } = null!;
}
