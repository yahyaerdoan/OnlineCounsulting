using OnlineConsulting.Entity.Concretions.BaseEntities;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Basket : BaseEntity
{
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }

    /// <summary>No User navigation here: User lives in a separate module/DbContext (Auth).</summary>
    public ICollection<BasketItem> BasketItems { get; set; } = [];
}
