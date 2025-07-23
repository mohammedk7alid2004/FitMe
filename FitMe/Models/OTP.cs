namespace FitMe.Models;

public class OTP
{

        public int Id { get; set; } 
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }

        public string UserId { get; set; }

        public ApplicationUser? User { get; set; }
    

}
