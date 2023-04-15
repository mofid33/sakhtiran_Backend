using System;

namespace MarketPlace.API.Data.Dtos.Dashboard
{
    public class GoodsRequestDto
    {
        public int GoodsId { get; set; }
        public string GoodsCode { get; set; }
        public string SerialNumber { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
        public int? ShopId { get; set; }
        public string ShopTitle { get; set; }
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int? BrandId { get; set; }
        public string BrandTitle { get; set; }
        public string Description { get; set; }
        public string GoodsTitle { get; set; }
    }
}