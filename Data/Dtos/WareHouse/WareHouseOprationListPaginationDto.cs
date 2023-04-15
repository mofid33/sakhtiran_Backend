namespace MarketPlace.API.Data.Dtos.WareHouse
{
    public class WareHouseOprationListPaginationDto
    {
        public int CategoryId { get; set; }
        public int GoodsId { get; set; }
        public int ShopId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}