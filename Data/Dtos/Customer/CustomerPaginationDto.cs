namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerPaginationDto
    {
        public int CustomerId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}