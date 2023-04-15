using MarketPlace.API.Helper;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerBalanceGetDto
    {
        public decimal AvailableBalance { get; set; }
        public Pagination<CustomerBalanceDto> CustomerBalance { get; set; }
    }
}