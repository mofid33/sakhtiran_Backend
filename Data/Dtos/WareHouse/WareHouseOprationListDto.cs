using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.WareHouse
{
    public class WareHouseOprationListDto
    {
        public int ProviderId { get; set; }
        public int FkShopId { get; set; }
        public string ShopTitle { get; set; }
        public int FkGoodsId { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string ImageUrl { get; set; }
        public bool? HasInventory { get; set; }
        public double? InventoryCount { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
    }
}