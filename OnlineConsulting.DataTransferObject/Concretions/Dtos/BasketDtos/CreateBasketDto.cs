namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.BasketDtos;

public class CreateBasketDto
{
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
