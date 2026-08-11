using OnlineConsulting.Entity.Concretions.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineConsulting.Entity.Concretions.Entities;

public class Service : BaseEntity
{
    public Guid CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DetailedDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool FeaturedArea { get; set; }
    public float DiscountRate { get; set; }
    public float TaxRate { get; set; }
    public decimal DiscountedPrice { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<ServiceImage> ServiceImages { get; set; } = [];
    public ICollection<BasketItem> BasketItems { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = []; // Yeni ilişki

    [NotMapped]
    public override string EntityName => "Service";
}
