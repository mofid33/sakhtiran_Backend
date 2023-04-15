namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsFormGetDto
    {
        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public bool HaveVariation { get; set; }
        public bool SaleWithCall { get; set; }
        public bool IsCommonGoods { get; set; }
    }
}