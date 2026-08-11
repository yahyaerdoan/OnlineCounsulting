using Microsoft.AspNetCore.Http;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;

namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

public class CreateServiceDto : IDto
{
    public Guid Id { get; set; }
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
    public List<string> ServiceImagesUrlList { get; set; } = [];
    public IFormFileCollection Images { get; set; } = null!;
}
