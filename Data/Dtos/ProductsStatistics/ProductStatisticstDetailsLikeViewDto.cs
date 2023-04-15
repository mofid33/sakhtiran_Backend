using System.Collections.Generic;

namespace MarketPlace.API.Data.Dtos.ProductsStatistics
{
    public class ProductStatisticsDetailsLikeViewDto
    {
        public int Registered  { get; set; }
        public int NotRegistered  { get; set; }
        public List<ProductStatisticsDetailsDto> details { get; set; }

    }
}