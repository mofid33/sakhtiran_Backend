namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsHomeDto
    {
        public GoodsHomeDto()
        {
            this.InCart = false;
        }

        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string GoodsImage { get; set; }
        public string ShopUrl { get; set; }
        public decimal Price { get; set; }
        public decimal Vat { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FinalPrice { get; set; }
        public long LikedCount { get; set; }
        public long ViewCount { get; set; }
        public double? SurveyScore { get; set; }
        public long? SurveyCount { get; set; }
        public bool? GoodsLiked { get; set; }
        public bool? ShopHaveMicroStore { get; set; }
        public double? InventoryCount { get; set; }
        public int ProviderId { get; set; }
        public bool ShippingPossibilities { get; set; }
        public bool InCart { get; set; }
        public bool HaveVariation { get; set; }
        public bool SaleWithCall { get; set; }
        public bool IsDownloadable { get; set; }
        public string GoodsBrand { get; set; }
        public string StoreName { get; set; }
        public bool HaveGuarantee { get; set; }
        public bool ReturningAllowed { get; set; }
        public int? GuaranteeMonthDuration { get; set; }
    }
}