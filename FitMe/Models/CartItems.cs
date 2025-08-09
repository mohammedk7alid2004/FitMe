namespace FitMe.Models;

public class CartItems
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int CartId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Price * Quantity;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public Cart ? Cart { get; set; }
    public Product? Product { get; set; }

}
