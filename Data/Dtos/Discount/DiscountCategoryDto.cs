namespace MarketPlace.API.Data.Dtos.Discount
{
    public class DiscountCategoryDto
    {
        public int AssingnedCategoryId { get; set; }
        public int FkCategoryId { get; set; }
        public bool Allowed { get; set; }
        public string CategoryPath { get; set; } 
        public long FkDiscountPlanId { get; set; }
    }
}