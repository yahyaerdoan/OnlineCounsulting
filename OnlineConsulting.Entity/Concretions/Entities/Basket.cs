using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Basket : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public User User { get; set; } = null!;
    public ICollection<BasketItem> BasketItems { get; set; } = [];
}
