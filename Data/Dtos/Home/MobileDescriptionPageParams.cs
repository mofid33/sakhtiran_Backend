namespace MarketPlace.API.Data.Dtos.Home
{
    public class MobileDescriptionPageParams
    {
        public string Type { get; set; }
        public string ContentType { get; set; }
        public int GoodsId { get; set; }
        public int ArticleId { get; set; }
        public string StoreName { get; set; }
        public string Lang { get; set; }
        public string Curr { get; set; }
    }
}