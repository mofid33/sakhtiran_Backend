using System;
using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Shop;

namespace MarketPlace.API.Data.Dtos.ProductsStatistics
{
    public class ProductStatisticsDetailsDto
    {
        public int CustomerId { get; set; }
        public string Date { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }

    }
}