namespace FitMe.Models
{
    public class Order: AuditableEntity
    {
        public int OrderId { get; set; }
        public Guid MerchantOrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string CurrencyCode { get; set; }= "EGP"; 
        public string Status { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
