using System;
using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsPaginationDto
    {
        public int CategoryId { get; set; }
        public int ShopId { get; set; }
        public int BrandId { get; set; }
        public bool Common { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Code { get; set; }
        public double PriceFrom { get; set; }
        public double PriceTo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int LastUpdateHoure { get; set; }
        public List<int> CatChilds { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int ProductType { get; set; }
    }
}