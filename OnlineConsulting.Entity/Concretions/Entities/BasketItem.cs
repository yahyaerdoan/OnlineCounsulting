using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class BasketItem : BaseEntity
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
