namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsDocumentDto
    {
        public int ImageId { get; set; }
        public int FkGoodsId { get; set; }
        public int? FkVarietyId { get; set; }
        public string DocumentUrl { get; set; }
    }
}