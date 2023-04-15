namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopFilesGetDto
    {
        public int FileId { get; set; }
        public int FkShopId { get; set; }
        public int FkDocumentTypeId { get; set; }
        public string DocumentTypeTitle { get; set; }
        public string FileUrl { get; set; }
        public int FkGroupd { get; set; }
        public string GroupTitle { get; set; }
        public int? FkPersonId { get; set; }
        public string PersonTitle { get; set; }

    }
}