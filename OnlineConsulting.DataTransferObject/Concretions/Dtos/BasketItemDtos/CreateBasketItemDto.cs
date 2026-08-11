using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketItemDtos;

public class CreateBasketItemDto
{
    public Guid ServiceId { get; set; }
    public Guid BasketId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public Service Service { get; set; } = null!;
    public Basket Basket { get; set; } = null!;
}
