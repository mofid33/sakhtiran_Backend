using System.Collections.Generic;
using MarketPlace.API.Data.Models;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopPostServiceDto
    {
        public decimal price { get; set; }
        public double weight { get; set; }
        public double length { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public string originCountryCode { get; set; }
        public string destinationCountryCode { get; set; }
        public string originCityName { get; set; }
        public string destinationCityName { get; set; }
        public string currency { get; set; }
        public string shopShippingCode { get; set; }
        public double qty { get; set; }
        public int shopId { get; set; }
        public TShopActivityCity  ShopMethodCity  { get; set; }
        public TShopActivityCountry  MethodShopCountry  { get; set; }
        public TShippingOnCountry  AjyalmethodCountry  { get; set; }
        public TShippingOnCity  AjyalmethodCity  { get; set; }

    }
}