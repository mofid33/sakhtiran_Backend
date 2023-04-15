using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Goods;

namespace MarketPlace.API.Data.Dtos.Order
{
    public class SalesListDto
    {
        public long ItemId { get; set; }
        public int GoodsId { get; set; }
        public string GoodsImage { get; set; }
        public string GoodsTitle { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public string ShopTitle { get; set; }
        public List<GoodsVarietyGetDto> Varity { get; set; }
        public double ItemCount { get; set; }
        public decimal FinalPrice { get; set; }
        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
        public string StatusColor { get; set; }
    }
}