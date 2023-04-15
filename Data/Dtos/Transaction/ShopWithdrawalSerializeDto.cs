using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Data.Dtos.Transaction
{
    public class ShopWithdrawalSerializeDto
    {
        public string ShopWithdrawal { get; set; }
        public IFormFile Document { get; set; }
    }
}