using System;

namespace MarketPlace.API.Data.Dtos.Shop
{
    public class ShopListWebPaginationDto
    {
        public string StoreName { get; set; }
        public int Sort { get; set; }
        public int CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public int ProvinceId { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}