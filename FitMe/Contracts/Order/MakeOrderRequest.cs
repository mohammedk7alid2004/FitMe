namespace FitMe.Contracts.Order
{
    public record MakeOrderRequest
    (
         string UserId,
         string FirstName ,
         string LastName,
         string Email 
         
    );
}
