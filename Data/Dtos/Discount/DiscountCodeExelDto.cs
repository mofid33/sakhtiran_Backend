namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCodeExelDto
    {
        public int CodeId { get; set; }
        public string DiscountCode { get; set; }
        public int MaxUse { get; set; }
        public int UseCount { get; set; }
        public bool? IsValid { get; set; }
    }
}