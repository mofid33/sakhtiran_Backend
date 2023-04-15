namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopFileDto
    {
        public int FileId { get; set; }
        public int FkShopId { get; set; }
        public int FkDocumentTypeId { get; set; }
        public string FileUrl { get; set; }
    }
}