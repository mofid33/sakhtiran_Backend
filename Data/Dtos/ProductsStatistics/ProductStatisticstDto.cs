using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.ProductsStatistics
{
    public class ProductStatisticsDto
    {
        public int GoodsId { get; set; }
        public string SerialNumber { get; set; }
        public string GoodsCode { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string CategoryTitle { get; set; }
        public string Vendor { get; set; }
        public string BrandTitle { get; set; }
        public string RegisterDate { get; set; }
        public string LastUpdate { get; set; }
        public string ShopList { get; set; }
        public int LikeCount { get; set; }
        public int VisitCount { get; set; }
        public long SellCount { get; set; }
        public int RegisterdView { get; set; }
        public int NotRegisterdView { get; set; }
        public List<ShopFormDto> Shops { get; set; }
        public ShopFormDto Shop { get; set; }

    }
}