namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCodePaginationDto
    {
        public string Code { get; set; }
        public string Plan { get; set; }
        public bool? Status { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}