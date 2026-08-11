namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;

public class CreateBasketDto
{
    public string UserId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
