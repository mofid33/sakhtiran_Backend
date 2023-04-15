namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCategoryGetDto
    {
        public int AssingnedCategoryId { get; set; }
        public long FkDiscountPlanId { get; set; }
        public int FkCategoryId { get; set; }
        public bool Allowed { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public string CategoryPath { get; set; }
    }
}