using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Basket : BaseEntity
{
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    // No User navigation: User lives in a separate module/DbContext (Auth).
    public ICollection<BasketItem> BasketItems { get; set; } = [];
}
