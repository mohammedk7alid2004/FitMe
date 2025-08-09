namespace FitMe.Models;

public class Cart
{
    public int CartId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser?User { get; set; }
    public ICollection<CartItems> Items { get; set; } = [];

}
