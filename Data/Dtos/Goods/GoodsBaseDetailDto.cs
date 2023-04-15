namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsBaseDetailDto
    {
        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string SerialNumber { get; set; }
        public string ShopName { get; set; }
        public int CategoryId { get; set; }
        public bool HaveVariation { get; set; }
        public bool SaleWithCall { get; set; }

    }
}