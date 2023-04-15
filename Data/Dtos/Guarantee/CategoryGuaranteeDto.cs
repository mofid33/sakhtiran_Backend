namespace MarketPlace.API.Data.Dtos.Guarantee
{
    public class CategoryGuaranteeDto
    {
        public int CategoryGuaranteeId { get; set; }
        public int FkCategoryId { get; set; }
        public int FkGuaranteeId { get; set; }
        public string CategoryTitle { get; set; }
    }
}