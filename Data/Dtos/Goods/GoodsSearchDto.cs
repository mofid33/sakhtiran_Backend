namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsSearchDto
    {
        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string CategoryTitle { get; set; }
        public int CategoryId { get; set; }
    }
}