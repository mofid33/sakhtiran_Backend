using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopBalanceGetDto
    {
        public decimal AvailableBalance { get; set; }
        public Pagination<ShopBalanceDto> ShopBalance { get; set; }
    }
}