namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopStatisticsDto
    {
        public decimal Balance { get; set; }
        public int Orders { get; set; }
        public double Sales { get; set; }
        public double Income { get; set; }
        public int ActiveOrder { get; set; }
        public int ActiveProduct { get; set; }
        public int OutOfStuck { get; set; }
    }
}