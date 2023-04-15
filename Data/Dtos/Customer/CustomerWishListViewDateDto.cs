using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.Customer
{
    public class CustomerWishListViewDateDto
    {
        public int GoodsId { get; set; }
        public string Title { get; set; }
        public string GoodsCode { get; set; }
        public string Image { get; set; }
        public string SerialNumber { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public List<ShopFormDto> Shop { get; set; }
        public string Date { get; set; }
    }
}