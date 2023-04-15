using System.Collections.Generic;
using MarketPlace.API.Data.Dtos.Brand;
using MarketPlace.API.Data.Dtos.Category;
using MarketPlace.API.Data.Dtos.Guarantee;
using MarketPlace.API.Data.Dtos.MeasurementUnit;

namespace MarketPlace.API.Data.Dtos.Goods
{
    public class GoodsFormDto
    {
        public List<WebsiteBrandDto> Brands { get; set; }
        public List<MeasurementUnitDto> MeasurementUnit { get; set; }
        public List<GuaranteeFormDto> Guarantee { get; set; }
        public GoodsDto Good { get; set; }
        public List<CategoryTreeView> Category { get; set; }
        public GoodsBaseDetailDto GoodsBaseDetail { get; set; }
    }
}